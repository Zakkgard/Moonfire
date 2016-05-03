namespace Moonfire.Core.Cryptography
{
    using Moonfire.Core.Networking;

    public class Authenticator
    {
        private readonly SecureRemotePassword srp;

        public Authenticator(SecureRemotePassword srp)
        {
            this.srp = srp;
        }

        public SecureRemotePassword SRP
        {
            get
            {
                return this.srp;
            }
        }

        public void WriteServerChallenge(PacketWriter packet)
        {
            packet.WriteBigInt(SRP.PublicEphemeralValueB, 32);
            packet.WriteBigIntLength(SRP.Generator, 1);

            // We will pad this out to 32 bytes.
            packet.WriteBigIntLength(SRP.Modulus, 32);
            packet.WriteBigInt(SRP.Salt);
            packet.WriteBigInt(SecureRemotePassword.RandomNumber(16));
        }

    }
}
