namespace Moonfire.Core.Networking
{
    using Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net;

    public abstract class ClientBase : IClient
    {
        // TODO: Get logger instance
        protected byte[] buffer = new byte[1024];
        protected Socket tcpSocket;

        protected ClientBase(IServer server)
        {
            this.tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Server = server;
        }

        protected uint BytesReceived { get; private set; }

        protected uint BytesSent { get; private set; }

        protected Socket TcpSocket
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

        protected IServer Server { get; set; }

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
                var socketArgs = new SocketAsyncEventArgs();
                socketArgs.SetBuffer();
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
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
