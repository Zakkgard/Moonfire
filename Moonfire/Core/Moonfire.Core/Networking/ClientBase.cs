namespace Moonfire.Core.Networking
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    using Moonfire.Core.Cryptography;
    using Moonfire.Core.Networking.Interfaces;

    public abstract class ClientBase : IClient
    {
        // TODO: Get logger instance
        protected static readonly BufferManager manager = new BufferManager(8192, 512);
        protected Socket tcpSocket;

        protected ClientBase(IServer server)
        {
            this.TcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
                if (this.tcpSocket != null && this.tcpSocket.Connected)
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
            var socketArgs = SocketArgsPool.GetSocketArgs();
            manager.SetBuffer(socketArgs);
            socketArgs.UserToken = this.TcpSocket;
            socketArgs.Completed += this.ProcessReceived;
            socketArgs.SocketFlags = SocketFlags.None;

            this.TcpSocket.ReceiveAsync(socketArgs);
        }

        private void ProcessReceived(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                if (args.BytesTransferred == 0)
                {
                    this.Server.DisconnectClient(this, true);
                }
                else
                {
                    if (this.OnReceive(args.Buffer))
                    {
                        manager.FreeBuffer(args);
                    }

                    //this.OnReceive(args.Buffer);

                    this.ResumeReceive();
                }
            }
            catch (Exception ex)
            {
                // TODO: Log Exception
                this.Server.DisconnectClient(this, true);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                args.Completed -= this.ProcessReceived;
                SocketArgsPool.ReleaseSocketArgs(args);
            }
        }

        public abstract bool OnReceive(byte[] buffer);

        //private void ReceiveAsyncComplete(object sender, SocketAsyncEventArgs args)
        //{
        //    this.ProcessReceive(args);
        //}

        public virtual void Send(byte[] packet, int offset, int length)
        {
            if (this.TcpSocket != null && this.TcpSocket.Connected)
            {
                var args = SocketArgsPool.GetSocketArgs();
                if (args != null)
                {
                    args.Completed += SendAsyncComplete;
                    args.SetBuffer(packet, offset, length);
                    args.UserToken = this.TcpSocket;
                    args.SocketFlags = SocketFlags.None;
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

        private void SendAsyncComplete(object sender, SocketAsyncEventArgs args)
        {
            args.Completed -= this.SendAsyncComplete;
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
                catch (Exception e)
                {
                    // TODO: Log and handle exceptions
                    Console.WriteLine(e.Message);
                }
            }
        }

        public abstract void Send(IOutgoingPacket packet);
    }
}
