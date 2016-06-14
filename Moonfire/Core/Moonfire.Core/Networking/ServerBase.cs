namespace Moonfire.Core.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    using Moonfire.Core.Networking.Interfaces;

    public delegate void ClientConnectedHandler(IClient client);
    public delegate void ClientDisconnectedHandler(IClient client, bool forced);

    public abstract class ServerBase : IServer
    {
        // TODO: get logger instance

        protected virtual ISet<IClient> Clients { get; set; }

        public virtual IPEndPoint EndPoint { get; set; }

        protected virtual TcpListener Listener { get; set; }

        public virtual bool IsRunning
        {
            get
            {
                if (this.Listener != null)
                {
                    return this.Listener.Server.IsBound;
                }

                return false;
            }
        }

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

        public virtual void Start()
        {
            try
            {
                if (!this.IsRunning)
                {
                    this.StartListening();
                }
            }
            catch (Exception e)
            {
                // TODO: log exception
                Console.WriteLine(e.Message);
                this.Stop();
            }
        }

        protected virtual void Stop()
        {
            if (this.IsRunning)
            {
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

        public virtual void DisconnectClient(IClient client, bool forced = true)
        {
            this.RemoveClient(client);

            try
            {
                this.OnClientDisconnected(client, forced);
                client.TcpSocket.Close();
            }
            catch (Exception e)
            {
                // TODO: Log exception
                Console.WriteLine(e.Message);
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
                    catch (Exception e)
                    {
                        // Log Exception
                        Console.WriteLine(e.Message);
                    }
                }

                this.Clients.Clear();
            }
        }

        protected virtual bool OnClientConnected(IClient client)
        {
            this.ClientConnected?.Invoke(client);
            return true;
        }

        protected virtual void OnClientDisconnected(IClient client, bool forced)
        {
            this.ClientDisconnected?.Invoke(client, forced);
            client.Dispose();
        }

        protected void StartListening()
        {
            if (!this.IsRunning)
            {
                this.Listener = new TcpListener(this.EndPoint.Address, this.EndPoint.Port);

                try
                {
                    this.Listener.Start();
                }
                catch (Exception e)
                {
                    // TODO: Log Exception
                    Console.WriteLine(e.Message);
                }

                //this.Listener.Listen(this.MaximumPendingConnections);
                this.StartAccepting();
            }
        }

        protected void StopListening()
        {
            try
            {
                this.Listener.Stop();
            }
            catch (Exception e)
            {

                // TODO: Log Exception

                Console.WriteLine(e.Message);
            }

            this.Listener = null;
        }

        protected void StartAccepting()
        {
            new Thread(this.ProcessAccept)
                .Start(200);
        }

        private async void ProcessAccept(object delay)
        {
            while (this.IsRunning)
            {
                Thread.Sleep((int) delay);

                if (this.Listener.Pending())
                {
                    IClient client = this.CreateClient();
                    client.TcpSocket = await this.Listener.AcceptSocketAsync();
                    if (client.TcpSocket != null)
                    {
                        client.BeginReceive();
                    }

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
