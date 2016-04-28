namespace Moonfire.Core.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    using Moonfire.Core.Networking.Interfaces;

    public delegate void ClientConnectedHandler(IClient client);
    public delegate void ClientDisconnectedHandler(IClient client, bool forced);

    public abstract class ServerBase : IServer
    {
        // TODO: get logger instance

        protected virtual ISet<IClient> Clients { get; set; }
        protected virtual IPEndPoint EndPoint { get; set; }
        protected virtual Socket Listener { get; set; }
        protected virtual bool IsRunning { get; set; }
        protected virtual int MaximumPendingConnections { get; set; }
        public int ClientCount
        {
            get
            {
                return this.Clients.Count;
            }
        }

        public event ClientConnectedHandler ClientConnected;
        public event ClientDisconnectedHandler ClientDisconnected;

        protected virtual void Start()
        {
            try
            {
                if (!this.IsRunning)
                {
                    this.IsRunning = true;
                    this.StartListening();
                }
            }
            catch (Exception)
            {
                // TODO: log exception
                this.Stop();
            }
        }

        protected virtual void Stop()
        {
            if (this.IsRunning)
            {
                this.IsRunning = false;
                this.RemoveAllClients();
                this.StopListening();
            }
        }

        protected abstract IClient CreateClient();

        protected virtual void RemoveClient(IClient client)
        {
            lock (this.Clients)
            {
                this.Clients.Remove(client);
            }
        }

        protected virtual void DisconnectClient(IClient client, bool forced = true)
        {
            this.RemoveClient(client);

            try
            {
                this.OnClientDisconnected(client, forced);
                client.Dispose();
            }
            catch (Exception)
            {
                // TODO: Log exception
            }
        }

        protected virtual void RemoveAllClients()
        {
            lock (this.Clients)
            {
                foreach (var client in this.Clients)
                {
                    try
                    {
                        this.OnClientDisconnected(client, true);
                    }
                    catch (Exception)
                    {
                        // Log Exception
                    }
                }

                this.Clients.Clear();
            }
        }

        protected virtual bool OnClientConnected(IClient client)
        {
            ClientConnectedHandler handler = this.ClientConnected;
            if (handler != null)
            {
                handler(client);
            }

            return true;
        }

        protected virtual void OnClientDisconnected(IClient client, bool forced)
        {
            ClientDisconnectedHandler handler = this.ClientDisconnected;
            if (handler != null)
            {
                handler(client, forced);
            }

            client.Dispose();
        }

        protected void StartListening()
        {
            if (!this.IsRunning)
            {
                this.Listener = new Socket(this.EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    this.Listener.Bind(this.EndPoint);
                }
                catch (Exception)
                {
                    // TODO: Log Exception
                }

                this.Listener.Listen(this.MaximumPendingConnections);
                this.StartAccepting(null);
            }
        }

        protected void StopListening()
        {
            try
            {
                this.Listener.Close();
            }
            catch (Exception ex)
            {
                // TODO: Log Exception
            }

            this.Listener = null;
        }

        protected void StartAccepting(SocketAsyncEventArgs acceptArgs)
        {
            if (acceptArgs != null)
            {
                // clear socket for reuse
                acceptArgs.AcceptSocket = null;
            }
            else
            {
                acceptArgs = new SocketAsyncEventArgs();
                acceptArgs.Completed += AcceptCompleted;
            }

            if (this.Listener.AcceptAsync(acceptArgs))
            {
                this.ProcessAccept(acceptArgs);
            }
        }

        private void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs args)
        {
            try
            {
                if (!this.IsRunning)
                {
                    return;
                }

                IClient client = this.CreateClient();
                client.TcpSocket = args.AcceptSocket;
                client.BeginReceive();

                this.StartAccepting(args);

                if (this.OnClientConnected(client))
                {
                    lock (this.Clients)
                    {
                        this.Clients.Add(client);
                    }
                }
                else
                {
                    client.TcpSocket.Shutdown(SocketShutdown.Both);
                    client.TcpSocket.Close();
                }
            }
            catch (SocketException e)
            {
                // TODO: Log Exception
            }
            catch (Exception e)
            {
                // TODO: Log Exception
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.IsRunning)
            {
                this.Stop();
            }
        }
    }
}
