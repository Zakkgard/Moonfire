namespace Moonfire.Core.Networking
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    using Moonfire.Core.Cryptography;
    
    public abstract class PacketWriter : BinaryWriter
    {
        public static Encoding DefaultEncoding = Encoding.UTF8;

        public PacketWriter(Stream stream)
            : base(stream)
        {

        }

        #region WriteByte
        
        public virtual void WriteByte(byte val)
        {
            Write(val);
        }

        public virtual void WriteByte(ushort val)
        {
            Write((byte)val);
        }
        
        public virtual void WriteByte(short val)
        {
            Write((byte)val);
        }
        
        public virtual void WriteByte(uint val)
        {
            Write((byte)val);
        }
        
        public virtual void WriteByte(int val)
        {
            Write((byte)val);
        }
        
        public virtual void WriteByte(bool val)
        {
            Write(val);
        }

        #endregion

        #region WriteShort
        
        public virtual void WriteShort(byte val)
        {
            Write((short)val);
        }
        
        public virtual void WriteShort(ushort val)
        {
            Write(val);
        }
        
        public virtual void WriteShort(short val)
        {
            Write(val);
        }
        
        public virtual void WriteShort(uint val)
        {
            Write((short)val);
        }
        
        public virtual void WriteShort(int val)
        {
            Write((short)val);
        }

        #endregion

        #region WriteInt
        
        public virtual void WriteInt(byte val)
        {
            Write((int)val);
        }
        
        public virtual void WriteInt(ushort val)
        {
            Write((int)val);
        }
        
        public virtual void WriteInt(short val)
        {
            Write((int)val);
        }
        
        public virtual void WriteInt(uint val)
        {
            Write(val);
        }
        
        public virtual void WriteInt(int val)
        {
            Write(val);
        }

        #endregion

        #region WriteFloat
        
        public virtual void WriteFloat(byte val)
        {
            Write((float)val);
        }
        
        public virtual void WriteFloat(ushort val)
        {
            Write((float)val);
        }
        
        public virtual void WriteFloat(short val)
        {
            Write((int)val);
        }
        
        public virtual void WriteFloat(uint val)
        {
            Write((float)val);
        }
        
        public virtual void WriteFloat(int val)
        {
            Write((float)val);
        }
        
        public virtual void WriteFloat(double val)
        {
            Write((float)val);
        }
        
        public virtual void WriteFloat(float val)
        {
            Write(val);
        }

        #endregion

        #region WriteUShort
        
        public virtual void WriteUShort(byte val)
        {
            Write((ushort)val);
        }
        
        public virtual void WriteUShort(ushort val)
        {
            Write(val);
        }
        
        public virtual void WriteUShort(short val)
        {
            Write((ushort)val);
        }
        
        public virtual void WriteUShort(uint val)
        {
            Write((ushort)val);
        }
        
        public virtual void WriteUShort(int val)
        {
            Write((ushort)val);
        }

        #endregion

        #region WriteUInt
        
        public virtual void WriteUInt(byte val)
        {
            Write((uint)val);
        }
        
        public virtual void WriteUInt(ushort val)
        {
            Write((uint)val);
        }
        
        public virtual void WriteUInt(short val)
        {
            Write((uint)val);
        }
        
        public virtual void WriteUInt(uint val)
        {
            Write(val);
        }
        
        public virtual void WriteUInt(int val)
        {
            Write((uint)val);
        }
        
        public virtual void WriteUInt(long val)
        {
            Write((uint)val);
        }

        #endregion

        #region WriteULong
        
        public virtual void WriteULong(byte val)
        {
            Write((ulong)val);
        }
        
        public virtual void WriteULong(ushort val)
        {
            Write((ulong)val);
        }
        
        public virtual void WriteULong(short val)
        {
            Write((ulong)val);
        }
        
        public virtual void WriteULong(uint val)
        {
            Write((ulong)val);
        }
        
        public virtual void WriteULong(int val)
        {
            Write((ulong)val);
        }
        
        public virtual void WriteULong(ulong val)
        {
            Write(val);
        }
        
        public virtual void WriteULong(long val)
        {
            Write((ulong)val);
        }

        #endregion

        public override void Write(string str)
        {
            Write(DefaultEncoding.GetBytes(str));
            Write((byte)0);
        }
        
        public virtual void WriteCString(string str)
        {
            Write(DefaultEncoding.GetBytes(str));
            Write((byte)0);
        }
        
        public virtual void WriteUTF8CString(string str)
        {
            Write(Encoding.UTF8.GetBytes(str));
            Write((byte)0);
        }
        
        public virtual void WriteBigInt(BigInteger bigInt)
        {
            byte[] data = bigInt.GetBytes();

            base.Write(data);
        }
        
        public virtual void WriteBigInt(BigInteger bigInt, int length)
        {
            byte[] data = bigInt.GetBytes(length);

            base.Write(data);
        }
        
        public virtual void WriteBigIntLength(BigInteger bigInt)
        {
            byte[] data = bigInt.GetBytes();

            base.Write((byte)data.Length);
            base.Write(data);
        }
        
        public virtual void WriteBigIntLength(BigInteger bigInt, int length)
        {
            byte[] data = bigInt.GetBytes(length);

            base.Write((byte)length);
            base.Write(data);
        }

        #region WriteShortBE
        
        public virtual void WriteShortBE(byte val)
        {
            Write(IPAddress.HostToNetworkOrder(val));
        }
        
        public virtual void WriteShortBE(short val)
        {
            Write(IPAddress.HostToNetworkOrder(val));
        }
        
        public virtual void WriteShortBE(int val)
        {
            Write(IPAddress.HostToNetworkOrder((byte)val));
        }

        #endregion

        #region WriteUShortBE
        public void WriteUShortBE(ushort val)
        {
            Write((byte)((val & 0xFF00) >> 8));
            Write((byte)(val & 0x00FF));
        }
        #endregion

        #region WriteIntBE
        
        public virtual void WriteIntBE(byte val)
        {
            Write(IPAddress.HostToNetworkOrder((int)val));
        }
        
        public virtual void WriteIntBE(short val)
        {
            Write(IPAddress.HostToNetworkOrder((int)val));
        }
        
        public virtual void WriteIntBE(int val)
        {
            Write(IPAddress.HostToNetworkOrder(val));
        }

        #endregion
        
        public void WriteLongBE(long val)
        {
            Write(IPAddress.HostToNetworkOrder(val));
        }
        
        public void InsertByteAt(byte value, long pos, bool returnOrigPos)
        {
            if (returnOrigPos)
            {
                long orig = BaseStream.Position;

                BaseStream.Position = pos;

                Write(value);

                BaseStream.Position = orig;
            }
            else
            {
                BaseStream.Position = pos;

                Write(value);
            }
        }

        public void InsertShortAt(short value, long pos, bool returnOrigPos)
        {
            if (returnOrigPos)
            {
                long orig = BaseStream.Position;

                BaseStream.Position = pos;

                Write(value);

                BaseStream.Position = orig;
            }
            else
            {
                BaseStream.Position = pos;

                Write(value);
            }
        }

        public void InsertIntAt(int value, long pos, bool returnOrigPos)
        {
            if (returnOrigPos)
            {
                long orig = BaseStream.Position;

                base.BaseStream.Position = pos;

                base.Write(value);
                base.BaseStream.Position = orig;
            }
            else
            {
                base.BaseStream.Position = pos;
                Write(value);
            }
        }
    }
}
