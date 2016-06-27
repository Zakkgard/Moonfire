namespace Moonfire.LogonServer
{
    using System.Collections.Generic;

    using Moonfire.Core.Networking;
    using Moonfire.Core.Networking.Interfaces;

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
