// Connection.cs
// <copyright file="Connection.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NetLib.Events;

namespace NetLib.Tcp.Server
{
    /// <summary>
    /// Represents a client connection to the server.
    /// </summary>
    internal class Connection
    {
        /// <summary>
        /// The TCP connection object.
        /// </summary>
        private TcpClient client;

        /// <summary>
        /// The underlying stream of data.
        /// </summary>
        private NetworkStream stream;

        /// <summary>
        /// The thread used to read incoming data.
        /// </summary>
        private Thread readThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection" /> class.
        /// </summary>
        /// <param name="client"> The connection to represent. </param>
        public Connection(TcpClient client)
        {
            this.client = client;
            this.stream = client.GetStream();

            this.readThread = new Thread(this.Read);
            this.readThread.Start();

            Console.WriteLine("Server: Client loaded");
        }

        /// <summary>
        /// Fires when the connection receives a message.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Sends the message to the server.
        /// </summary>
        /// <param name="message"> The message to be sent. </param>
        public void Send(string message)
        {
            StreamUtil.Send(this.stream, message);
        }

        /// <summary>
        /// Manages and fires the <see cref="MessageReceived" /> event.
        /// </summary>
        /// <param name="e"> The event arguments to fire with. </param>
        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            EventHandler<MessageReceivedEventArgs> handler = this.MessageReceived;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Reads data from the connection.
        /// </summary>
        /// <remarks>
        /// This is a blocking method.
        /// </remarks>
        private void Read()
        {
            StreamUtil.Read(this.stream, this.Message);

            // If the read function fails, close the connection
            this.client.Close();
        }

        /// <summary>
        /// A delegate function called when a message is received.
        /// </summary>
        /// <remarks>
        /// This is the delegate callback called by StreamUtils.Read on this object's read thread.
        /// </remarks>
        /// <param name="msg"> The message received. </param>
        private void Message(string msg)
        {
            this.OnMessageReceived(new MessageReceivedEventArgs(msg, (this.client.Client.RemoteEndPoint as IPEndPoint).Address));
        }
    }
}
