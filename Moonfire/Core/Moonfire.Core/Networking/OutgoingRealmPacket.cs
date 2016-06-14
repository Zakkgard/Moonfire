using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonfire.Core.Constants;
using Moonfire.Core.Constants.Auth;

namespace Moonfire.Core.Networking
{
    public class OutgoingRealmPacket : OutgoingPacket
    {
        public OutgoingRealmPacket(WorldOpCode opcode) 
            : base(opcode)
        {
            this.Position += 2;
            this.Write((ushort)opcode);
        }
    }
}
