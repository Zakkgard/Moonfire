namespace Moonfire.Core.Networking
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    using Moonfire.Core.Networking.Interfaces;

    public abstract class ClientBase : IClient
    {
        // TODO: Get logger instance
        protected static readonly BufferManager manager = new BufferManager(8192, 512);
        protected Socket tcpSocket;

        protected ClientBase(IServer server)
        {
            this.tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Server = server;
        }
        
        public Socket TcpSocket
        {
            get
            {
                return this.tcpSocket;
            }
            set
            {
                if (this.TcpSocket != null && this.TcpSocket.Connected)
                {
                    this.TcpSocket.Shutdown(SocketShutdown.Both);
                    this.TcpSocket.Close();
                }

                if (value != null)
                {
                    this.tcpSocket = value;
                }
            }
        }

        public IServer Server { get; set; }

        public IPAddress ClientAddress
        {
            get
            {
                return this.TcpSocket != null && this.TcpSocket.RemoteEndPoint != null ? ((IPEndPoint)this.TcpSocket.RemoteEndPoint).Address : null;
            }
        }

        public int Port
        {
            get
            {
                return this.TcpSocket != null && this.TcpSocket.RemoteEndPoint != null ? ((IPEndPoint)this.TcpSocket.RemoteEndPoint).Port : -1;
            }
        }

        protected uint BytesReceived { get; private set; }

        protected uint BytesSent { get; private set; }

        public bool IsConnected
        {
            get
            {
                return this.TcpSocket != null && this.TcpSocket.Connected;
            }
        }

        public void BeginReceive()
        {
            this.ResumeReceive();
        }

        private void ResumeReceive()
        {
            if (this.IsConnected)
            {
                var socketArgs = SocketArgsPool.GetSocketArgs();
                manager.SetBuffer(socketArgs);
                socketArgs.UserToken = this;
                socketArgs.Completed += this.ReceiveAsyncComplete;

                if (this.TcpSocket.ReceiveAsync(socketArgs))
                {
                    this.ProcessReceive(socketArgs);
                }
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs args)
        {
            try
            {
                if (args.BytesTransferred == 0)
                {
                    this.Server.DisconnectClient(this, true);
                }
                else
                {
                    unchecked
                    {
                        this.BytesReceived += (uint)args.BytesTransferred;
                    }

                    if (this.OnReceive(args.Buffer))
                    {
                        manager.FreeBuffer(args);
                    }

                    this.ResumeReceive();
                }
            }
            catch (Exception)
            {
                // TODO: Log Exception
                this.Server.DisconnectClient(this, true);
            }
            finally
            {
                args.Completed -= this.ReceiveAsyncComplete;
                SocketArgsPool.ReleaseSocketArgs(args);
            }
        }

        public abstract bool OnReceive(byte[] buffer);

        private void ReceiveAsyncComplete(object sender, SocketAsyncEventArgs args)
        {
            this.ProcessReceive(args);
        }

        public virtual void Send(byte[] packet, int offset, int length)
        {
            if (this.TcpSocket != null && this.TcpSocket.Connected)
            {
                var args = SocketArgsPool.GetSocketArgs();
                if (args != null)
                {
                    args.Completed += SendAsyncComplete;
                    args.SetBuffer(packet, offset, length);
                    args.UserToken = this;
                    this.TcpSocket.SendAsync(args);

                    unchecked
                    {
                        this.BytesSent += (uint)length;
                    }
                }
                else
                {
                    // TODO: Log an error
                }
            }
        }

        private static void SendAsyncComplete(object sender, SocketAsyncEventArgs args)
        {
            args.Completed -= SendAsyncComplete;
            SocketArgsPool.ReleaseSocketArgs(args);
        }

        public void Connect(string host, int port)
        {
            Connect(IPAddress.Parse(host), port);
        }

        public void Connect(IPAddress addr, int port)
        {
            if (this.TcpSocket != null)
            {
                if (this.TcpSocket.Connected)
                {
                    this.TcpSocket.Disconnect(true);
                }

                this.TcpSocket.Connect(addr, port);

                this.BeginReceive();
            }
        }

        ~ClientBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.TcpSocket != null && this.TcpSocket.Connected)
            {
                try
                {
                    this.TcpSocket.Shutdown(SocketShutdown.Both);
                    this.TcpSocket.Close();
                    this.TcpSocket = null;
                }
                catch (Exception ex)
                {
                    // TODO: Log and handle exceptions
                }
            }
        }
    }
}
