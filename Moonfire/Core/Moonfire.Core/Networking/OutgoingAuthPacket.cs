using Moonfire.Core.Constants.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonfire.Core.Networking
{
    public class OutgoingAuthPacket : OutgoingPacket
    {
        public OutgoingAuthPacket(AuthenticationCmd opcode)
            : base(opcode)
        {
            base.WriteByte((byte)opcode);
        }
    }
}
