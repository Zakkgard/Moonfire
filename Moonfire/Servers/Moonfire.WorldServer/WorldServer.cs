using Moonfire.Core.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonfire.Core.Networking.Interfaces;

namespace Moonfire.WorldServer
{
    class WorldServer : ServerBase
    {
        public WorldServer()
        {
            this.Clients = new HashSet<IClient>();
        }

        protected override IClient CreateClient()
        {
            return new RealmClient(this);
        }
    }
}
