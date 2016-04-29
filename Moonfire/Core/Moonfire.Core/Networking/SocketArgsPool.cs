namespace Moonfire.Core.Networking
{
    using System.Net.Sockets;

    using Moonfire.Core.Collections;

    public static class SocketArgsPool
    {
        private static readonly ObjectPool<SocketAsyncEventArgs> objectPool = new ObjectPool<SocketAsyncEventArgs>(() => new SocketAsyncEventArgs());

        public static SocketAsyncEventArgs GetSocketArgs()
        {
            return objectPool.GetObject();
        }

        public static void ReleaseSocketArgs(SocketAsyncEventArgs args)
        {
            if (args != null)
            {
                Cleanup(args);
                objectPool.PutObject(args);
            }
        }

        private static void Cleanup(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;
            args.SetBuffer(null, 0, 0);
            args.BufferList = null;
            args.DisconnectReuseSocket = false;
            args.RemoteEndPoint = null;
            args.SendPacketsElements = null;
            args.SendPacketsFlags = 0;
            args.SendPacketsSendSize = 0;
            args.SocketError = 0;
            args.SocketFlags = 0;
            args.UserToken = null;
        }
    }
}
