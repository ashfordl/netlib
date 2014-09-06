// Server.cs
// <copyright file="Server.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetLib.Events;

namespace NetLib.Tcp
{
    /// <summary>
    /// Represents a TCP server.
    /// </summary>
    public class Server
    {
        private readonly int port;

        private TcpListener listener;

        private Thread listenThread;

        private List<Client> connections;

        /// <summary>
        /// Initializes a new instance of the <see cref="Server" /> class.
        /// </summary>
        /// <param name="port"> The port number of the server. </param>
        public Server(int port)
        {
            if (port < 1024 || port > 65535)
            {
                throw new ArgumentOutOfRangeException("port", "Port must be between 1 and 65536");
            }
            else
            {
                this.port = port;
            }

            this.connections = new List<Client>();
        }

        /// <summary>
        /// Fires when the connection receives a message.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Fires when the remote becomes disconnected.
        /// </summary>
        public event EventHandler<DisconnectedEventArgs> ClientDisconnected;

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

        /// <summary>
        /// Sends the passed message to every connected client.
        /// </summary>
        /// <param name="message"> The message to send. </param>
        public void MessageAllClients(string message)
        {
            lock (this.connections)
            {
                foreach (Client client in this.connections)
                {
                    client.Send(message);
                }
            }
        }

        /// <summary>
        /// Manages and fires the <see cref="MessageReceived" /> event.
        /// </summary>
        /// <param name="origin"> The Client object that first fired the event. </param>
        /// <param name="e"> The event arguments to fire with. </param>
        protected virtual void OnMessageReceived(object origin, MessageReceivedEventArgs e)
        {
            EventHandler<MessageReceivedEventArgs> handler = this.MessageReceived;

            if (handler != null)
            {
                handler(origin, e);
            }
        }

        /// <summary>
        /// Manages and fires the <see cref="ClientDisconnected" /> event.
        /// </summary>
        /// <param name="origin"> The Client object that first fired the event. </param>
        /// <param name="e"> The event arguments to fire with. </param>
        protected virtual void OnClientDisconnected(object origin, DisconnectedEventArgs e)
        {
            EventHandler<DisconnectedEventArgs> handler = this.ClientDisconnected;

            if (handler != null)
            {
                handler(origin, e);
            }
        }

        /// <summary>
        /// Handles the connection of a new client.
        /// </summary>
        /// <param name="obj"> The client connection object. </param>
        protected virtual void HandleClientConnected(object obj)
        {
            TcpClient client = (TcpClient)obj;

            // Create new connection
            Client connect = new Client(client);

            // Subscribe to events
            connect.MessageReceived += this.Client_MessageReceived;
            connect.Disconnected += this.Client_Disconnected;
            
            // Add the connection to the master list
            lock (this.connections)
            {
                this.connections.Add(connect);
            }
        }

        /// <summary>
        /// Handles the MessageReceived event for each Client
        /// </summary>
        /// <param name="sender"> The object that raised the event. </param>
        /// <param name="e"> The event arguments. </param>
        protected virtual void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            this.OnMessageReceived(sender, e);
        }

        /// <summary>
        /// Handles the Disconnected event for each Client
        /// </summary>
        /// <param name="sender"> The object that raised the event. </param>
        /// <param name="e"> The event arguments. </param>
        protected virtual void Client_Disconnected(object sender, DisconnectedEventArgs e)
        {
            this.OnClientDisconnected(sender, e);

            lock (this.connections)
            {
                this.connections.Remove((Client)sender);
            }
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
    }
}
