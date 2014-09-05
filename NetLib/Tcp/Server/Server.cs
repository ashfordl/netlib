// Server.cs
// <copyright file="Server.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetLib.Events;

namespace NetLib.Tcp.Server
{
    /// <summary>
    /// Represents a TCP server.
    /// </summary>
    public class Server
    {
        private readonly int port;

        private TcpListener listener;

        private Thread listenThread;

        private List<Connection> connections;

        /// <summary>
        /// Initializes a new instance of the <see cref="Server" /> class.
        /// </summary>
        /// <param name="port"> The port number of the server. </param>
        public Server(int port)
        {
            if (port < 1024 || port > 65535)
            {
                throw new ArgumentOutOfRangeException("value", "Port must be between 1 and 65536");
            }
            else
            {
                this.port = port;
            }

            this.connections = new List<Connection>();
        }

        /// <summary>
        /// Gets the port number of the server.
        /// </summary>
        public int Port
        {
            get
            {
                return this.port;
            }
        }

        /// <summary>
        /// Runs the server.
        /// </summary>
        public void Serve()
        {
            this.listener = new TcpListener(IPAddress.Any, this.Port);
            this.listenThread = new Thread(new ThreadStart(this.Listen));
            
            this.listenThread.Start();
        }

        private void Listen()
        {
            this.listener.Start();

            while (true)
            {
                // Block until a client connects
                TcpClient client = this.listener.AcceptTcpClient();

                // Create a new client thread
                Thread clientThread = new Thread(new ParameterizedThreadStart(this.HandleClientConnected));
                clientThread.Start(client);
            }
        }

        private void HandleClientConnected(object obj)
        {
            TcpClient client = (TcpClient)obj;

            // Create new connection
            Connection connect = new Connection(client);

            // Subscribe to events
            connect.MessageReceived += this.MessageReceived;
            connect.RemoteDisconnected += this.RemoteDisconnected;
            
            // Add the connection to the master list
            lock (this.connections)
            {
                this.connections.Add(connect);
            }
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine("Server: Message received \"" + e.Message + "\"");
        }

        private void RemoteDisconnected(object sender, RemoteDisconnectedEventArgs e)
        {
            Console.WriteLine("Server: Client disconnected. IP = " + e.RemoteIP);
        }
    }
}
