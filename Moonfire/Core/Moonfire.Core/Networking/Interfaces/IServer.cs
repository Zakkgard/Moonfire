namespace Moonfire.Core.Networking.Interfaces
{
    using System;

    public interface IServer : IDisposable
    {
        void DisconnectClient(IClient client, bool forced);
    }
}
