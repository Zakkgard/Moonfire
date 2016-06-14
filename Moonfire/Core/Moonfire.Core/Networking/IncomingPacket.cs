namespace Moonfire.Core.Networking
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Moonfire.Core.Cryptography;
    using Interfaces;

    public abstract class IncomingPacket : BinaryReader, IPacket
    {
        public static Encoding DefaultEncoding = Encoding.UTF8;

        protected IncomingPacket(byte[] packet, int offset, int length)
            : base(new MemoryStream(packet, offset, length), DefaultEncoding)
        {
        }

        public int Position
        {
            get
            {
                return (int)this.BaseStream.Position;
            }
            set
            {
                this.BaseStream.Position = value;
            }
        }

        public int Length
        {
            get
            {
                return (int)this.BaseStream.Length;
            }
        }

        public int RemainingLength
        {
            get
            {
                return (int)(this.BaseStream.Length - this.BaseStream.Position);
            }
        }

        private bool EnsureData(int length)
        {
            if (this.Length - this.Position < length)
            {
                // TODO: Log errors.
                return false;
            }

            return true;
        }

        public ushort ReadUInt16BE()
        {
            return (ushort)((base.ReadByte() << 8) | base.ReadByte());
        }

        public float ReadFloat()
        {
            return base.ReadSingle();
        }

        public string ReadCString()
        {
            byte tempByte;
            var chrBuffer = new List<byte>();

            while ((tempByte = base.ReadByte()) != 0)
            {
                chrBuffer.Add(tempByte);
            }

            return DefaultEncoding.GetString(chrBuffer.ToArray());
        }

        public string ReadPascalString()
        {
            int size = ReadByte();

            if (!this.EnsureData(size))
            {
                return string.Empty;
            }

            return new string(ReadChars(size));
        }

        public string ReadReversedString()
        {
            byte tempByte;
            var chrBuffer = new List<byte>();

            while ((tempByte = base.ReadByte()) != 0)
            {
                chrBuffer.Add(tempByte);
            }

            char[] stringChrs = DefaultEncoding.GetChars(chrBuffer.ToArray());
            stringChrs.Reverse();

            return new string(stringChrs);
        }

        public BigInteger ReadBigInteger(int length)
        {
            if (length < 1)
            {
                throw new ArgumentOutOfRangeException("length", "BigInteger length must be greater than zero!");
            }

            if (!this.EnsureData(length))
            {
                return new BigInteger(0);
            }

            byte[] data = ReadBytes(length);

            if (data.Length < length)
            {
                return new BigInteger(0);
            }

            return new BigInteger(data);
        }

        public BigInteger ReadBigIntegerLengthValue()
        {
            byte length = base.ReadByte();

            return (length != 0 ? ReadBigInteger(length) : new BigInteger());
        }

        public void SkipBytes(int num)
        {
            base.BaseStream.Seek(num, SeekOrigin.Current);
        }

        public long Seek(int offset, SeekOrigin origin)
        {
            return base.BaseStream.Seek(offset, origin);
        }
    }
}
