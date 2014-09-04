// Server.cs
// <copyright file="Server.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetLib.Events;

namespace NetLib.Server
{
    /// <summary>
    /// Represents a TCP server.
    /// </summary>
    public class Server
    {
        /// <summary>
        /// The TCP listener to watch for incoming connections.
        /// </summary>
        private TcpListener listener;

        /// <summary>
        /// The thread to listen for incoming connections.
        /// </summary>
        private Thread listenThread;

        /// <summary>
        /// A list of all current connections.
        /// </summary>
        private List<Connection> connections;

        /// <summary>
        /// The internal storage of the port number.
        /// </summary>
        private int port;

        /// <summary>
        /// Initializes a new instance of the <see cref="Server" /> class.
        /// </summary>
        /// <param name="port"> The port number of the server. </param>
        public Server(int port)
        {
            this.Port = port;

            this.connections = new List<Connection>();
        }

        /// <summary>
        /// Gets or sets the port number of the server.
        /// </summary>
        public int Port
        {
            get
            {
                return this.port;
            }

            protected set
            {
                if (value < 1024 || value > 65535)
                {
                    throw new ArgumentOutOfRangeException("value", "Port must be between 1 and 65536");
                }

                this.port = value;
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

        /// <summary>
        /// Listens for any incoming connections.
        /// </summary>
        /// <remarks>
        /// This is a blocking method.
        /// </remarks>
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

        /// <summary>
        /// Handles a new client connection.
        /// </summary>
        /// <param name="obj"> The client connection object. </param>
        private void HandleClientConnected(object obj)
        {
            TcpClient client = (TcpClient)obj;

            // Create new connection
            Connection connect = new Connection(client);

            // Subscribe to events
            connect.MessageReceived += this.MessageReceived;
            
            // Add the connection to the master list
            lock (this.connections)
            {
                this.connections.Add(connect);
            }
        }

        /// <summary>
        /// Handles the message received event.
        /// </summary>
        /// <param name="sender"> The event sender. </param>
        /// <param name="e"> The event arguments. </param>
        private void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine("Server: Message received \"" + e.Message + "\"");
        }
    }
}
