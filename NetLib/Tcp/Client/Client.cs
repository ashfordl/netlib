// Client.cs
// <copyright file="Client.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetLib.Tcp.Client
{
    /// <summary>
    /// Represents a connection to a <see cref="NetLib.Tcp.Server.Server" />.
    /// </summary>
    public class Client
    {
        private TcpClient tcp;

        private NetworkStream stream;

        private Thread readThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        /// <param name="ipep"> The <see cref="IPEndPoint" /> of the server. </param>
        public Client(IPEndPoint ipep)
        {
            this.tcp = new TcpClient();
            this.tcp.Connect(ipep);

            this.stream = this.tcp.GetStream();

            this.InitReadThread();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        /// <param name="ip"> The <see cref="IPAdress" /> of the server. </param>
        /// <param name="port"> The port number of the server. </param>
        public Client(IPAddress ip, int port)
        {
            this.tcp = new TcpClient();
            this.tcp.Connect(new IPEndPoint(ip, port));

            this.stream = this.tcp.GetStream();

            this.InitReadThread();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        /// <param name="hostname"> The host name of the server. </param>
        /// <param name="port"> The port number of the server. </param>
        public Client(string hostname, int port)
        {
            this.tcp = new TcpClient(hostname, port);
            this.stream = this.tcp.GetStream();

            this.InitReadThread();
        }

        public void Disconnect()
        {
            this.readThread.Abort();
            this.tcp.Close();
        }

        /// <summary>
        /// Sends the message to the server.
        /// </summary>
        /// <param name="message"> The message to be sent. </param>
        public void Send(string message)
        {
            StreamUtil.Send(this.stream, message);
        }

        /// <summary>
        /// Sends the payload to the server.
        /// </summary>
        /// <param name="payload"> The payload to be sent. </param>
        public void Send(byte[] payload)
        {
            StreamUtil.Send(this.stream, payload);
        }

        private void InitReadThread()
        {
            this.readThread = new Thread(this.Read);
            this.readThread.Start();
        }

        private void Read()
        {
            StreamUtil.Read(this.stream, this.Message);

            // If the read function fails, close the connection
            this.tcp.Close();
        }

        private void Message(string msg)
        {
            // this.OnMessageReceived(new MessageReceivedEventArgs(msg, (this.client.Client.RemoteEndPoint as IPEndPoint).Address));
            Console.WriteLine("Client: message received");
        }
    }
}
