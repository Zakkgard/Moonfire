namespace Moonfire.WorldServer
{
    using System;
    using System.Collections.Generic;

    using Moonfire.Core.Networking;
    using Moonfire.Core.Networking.Interfaces;
    
    public class WorldServer : ServerBase
    {
        public readonly byte[] Seed = BitConverter.GetBytes(new Random().Next());

        public WorldServer()
        {
            this.Clients = new HashSet<IClient>();
        }

        protected override IClient CreateClient()
        {
            return new RealmClient(this);
        }

        protected override bool OnClientConnected(IClient client)
        {
            base.OnClientConnected(client);
            LoginHandler.SendAuthChallenge(client);
            return true;
        }
    }
}
