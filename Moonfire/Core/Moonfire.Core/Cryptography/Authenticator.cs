namespace Moonfire.Core.Cryptography
{
    using Moonfire.Core.Networking;

    public class Authenticator
    {
        private readonly SRP6 srp;

        public Authenticator(SRP6 srp)
        {
            this.srp = srp;
        }

        public SRP6 SRP
        {
            get
            {
                return this.srp;
            }
        }

        public void WriteServerChallenge(PacketWriter packet)
        {
            packet.WriteBigInt(this.SRP.PublicEphemeralB);
            packet.WriteBigIntLength(this.SRP.Generator, 1);
            packet.WriteBigIntLength(this.SRP.Modulus, 32);
            packet.WriteBigInt(this.SRP.Salt);
            packet.WriteBigInt(this.SRP.Unknown);
        }

    }
}
