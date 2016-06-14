using Moonfire.Core.Constants.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonfire.Core.Networking.Interfaces
{
    public interface IOutgoingPacket : IPacket
    {
        object PacketId { get; set; }

        byte[] GetFinalizedPacket();
    }
}
