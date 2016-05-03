namespace Moonfire.Core.Cryptography
{
    using System;
    using System.Text;

    public class SRP6
    {
        private SRPParameters m_srpParams = SRPParameters.Defaults;
        private readonly BigInteger m_salt = RandomNumber();
        private BigInteger m_sessionKey;
        private readonly BigInteger m_secretEphemeralValueB = RandomNumber();
        private readonly BigInteger unknown = RandomNumber(16);

        public SRP6(string username, BigInteger identityHash)
        {
            this.Username = username;
            this.IdentityHash = identityHash;
        }

        public string Username { get; set; }

        public BigInteger IdentityHash { get; set; }

        public BigInteger CredentialsHash
        {
            get
            {
                return Hash(this.Salt, this.IdentityHash);
            }
        }

        public SRPParameters Parameters
        {
            get { return this.m_srpParams; }
        }

        public BigInteger Unknown { get { return this.unknown; } }

        public BigInteger Modulus
        {
            get { return Parameters.Modulus; }
        }

        public BigInteger Generator
        {
            get { return Parameters.Generator; }
        }

        public BigInteger GeneratorModuloN
        {
            get { return Generator.ModPow(m_secretEphemeralValueB, this.Modulus); }
        }

        public BigInteger Multiplier
        {
            get { return Parameters.Multiplier; }
        }

        public BigInteger Salt
        {
            get
            {
                return m_salt;
            }
        }

        public BigInteger Verifier
        {
            get
            {
                return this.Generator.ModPow(this.CredentialsHash, this.Modulus);
            }
        }

        public BigInteger Scrambler { get { return Hash(PublicEphemeralA, PublicEphemeralB); } }

        public BigInteger PublicEphemeralA { get; set; }

        public BigInteger PublicEphemeralB
        {
            get
            {
                return ((this.Verifier * 3) + this.GeneratorModuloN) % this.Modulus;
            }
        }

        // S as computed by the server
        public BigInteger SessionKeyRaw
        {
            get
            {
                // (A * (v.ModExp(u, N))).ModExp(b, N);
                return (PublicEphemeralA * (Verifier.ModPow(Scrambler, Modulus))).ModPow(m_secretEphemeralValueB, Modulus);
            }
        }

        public BigInteger SessionKey
        {
            get
            {
                byte[] t = SessionKeyRaw.GetBytes(32);
                byte[] t1 = new byte[16];
                byte[] vK = new byte[40];

                for (int i = 0; i < 16; i++)
                {
                    t1[i] = t[i * 2];
                }

                var hashedT1 = SRP6.stHash(t1).GetBytes();
                for (int i = 0; i < 20; i++)
                {
                    vK[i * 2] = hashedT1[i];
                }

                for (int i = 0; i < 16; ++i)
                {
                    t1[i] = t[i * 2 + 1];
                }

                hashedT1 = SRP6.stHash(t1).GetBytes();
                for (int i = 0; i < 20; ++i)
                {
                    vK[i * 2 + 1] = hashedT1[i];
                }

                return new BigInteger(vK, 40);
            }
        }


        // M
        public BigInteger ClientSessionKeyProof
        {
            get
            {
                return
                    Hash(Hash(Modulus) ^ Hash(Generator), Hash(Username), Salt, PublicEphemeralA,
                         PublicEphemeralB, SessionKey);
            }
        }

        public BigInteger ServerSessionKeyProof
        {
            get { return Hash(PublicEphemeralA, ClientSessionKeyProof, SessionKey); }
        }

        // Is M1 == M ?
        public bool IsClientProofValid(BigInteger client_proof)
        {
            return ClientSessionKeyProof == client_proof;
        }

        public static BigInteger RandomNumber()
        {
            return RandomNumber(SRPParameters.KeyLength);
        }

        public static BigInteger RandomNumber(uint size)
        {
            var buffer = new byte[size];

            SRPParameters.RandomGenerator.GetBytes(buffer);

            // Must make sure the most significant byte is not zero
            if (buffer[0] == 0)
                buffer[0] = 1;

            return new BigInteger(buffer);
        }

        public static byte[] GenerateCredentialsHash(string username, string password)
        {
            // Account-names are always upper case
            var buf = SRPParameters.Hash.ComputeHash(
                Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username.ToUpper(), password.ToUpper())));

            return buf;
        }

        public BigInteger Hash(params HashUtilities.HashDataBroker[] brokers)
        {
            return HashUtilities.HashToBigInteger(SRPParameters.Hash, brokers);
        }

        public static BigInteger stHash(params HashUtilities.HashDataBroker[] brokers)
        {
            return HashUtilities.HashToBigInteger(SRPParameters.Hash, brokers);
        }
    }
}
