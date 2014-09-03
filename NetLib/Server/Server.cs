// Server.cs
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetLib.Server
{
    public class Server
    {
        private int _port;
        public int Port
        {
            get
            {
                return this._port;
            }
            protected set
            {
                if (value < 1024 || value > 65535)
                {
                    throw new ArgumentOutOfRangeException("value", "Port must be between 1 and 65536");
                }

                this._port = value;
            }
        }

        private TcpListener listener;
        private Thread listenThread;

        private List<Connection> connections;

        public Server(int port)
        {
            this.Port = port;

            this.connections = new List<Connection>();
        }

        public void Serve()
        {
            this.listener = new TcpListener(IPAddress.Any, this.Port);
            this.listenThread = new Thread(new ThreadStart(Listen));
            
            this.listenThread.Start();
        }

        protected void Listen()
        {
            this.listener.Start();

            while (true)
            {
                // Block until a client connects
                TcpClient client = this.listener.AcceptTcpClient();

                // Create a new client thread
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientConnected));
                clientThread.Start(client);
            }
        }

        protected void HandleClientConnected(object obj)
        {
            TcpClient client = (TcpClient)obj;

            Connection connect = new Connection(client);

            lock (this.connections)
            {
                this.connections.Add(connect);
            }
        }
    }
}
