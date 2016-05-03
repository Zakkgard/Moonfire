namespace Moonfire.Core.Networking.Interfaces
{
    using Cryptography;
    using System;
    using System.Net;
    using System.Net.Sockets;

    public interface IClient : IDisposable
    {

        IServer Server { get; set; }

        /// <summary>
        /// Gets the IP address of the client.
        /// </summary>
        IPAddress ClientAddress { get; }

        /// <summary>
        /// Gets the port the client is communicating on.
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Gets/Sets the socket this client is using for TCP communication.
        /// </summary>
        Socket TcpSocket { get; set; }

        bool IsConnected { get; }

        Authenticator Authenticator { get; }

        /// <summary>
        /// Begins asynchronous TCP receiving for this client.
        /// </summary>
		void BeginReceive();

        /// <summary>
        /// Asynchronously sends a packet of data to the client.
        /// </summary>
        /// <param name="packet">An array of bytes containing the packet to be sent.</param>
        /// <param name="length">The number of bytes to send starting at offset.</param>
        /// <param name="offset">The offset into packet where the sending begins.</param>
		void Send(byte[] packet, int offset, int length);

        /// <summary>
        /// Asynchronously sends a packet of auth data to the client.
        /// </summary>
        /// <param name="packet">An instance of the auth packet to be sent.</param>
		void Send(OutgoingAuthPacket packet);

        /// <summary>
        /// Connects the client to the server at the specified address and port.
        /// </summary>
        /// <remarks>This function uses IPv4.</remarks>
        /// <param name="host">The IP address of the server to connect to.</param>
        /// <param name="port">The port to use when connecting to the server.</param>
        void Connect(string host, int port);

        /// <summary>
        /// Connects the client to the server at the specified address and port.
        /// </summary>
        /// <remarks>This function uses IPv4.</remarks>
        /// <param name="addr">The IP address of the server to connect to.</param>
        /// <param name="port">The port to use when connecting to the server.</param>
        void Connect(IPAddress addr, int port);
    }
}
