using Moonfire.Core.Networking;
using Moonfire.Core.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonfire.LogonServer
{
    public class LogonServer : ServerBase
    {
        public LogonServer()
        {
            this.Clients = new HashSet<IClient>();
        }

        protected override IClient CreateClient()
        {
            return new AuthClient(this);
        }
    }
}
