// Client.cs
// <copyright file="Client.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NetLib.Events;

namespace NetLib.Tcp
{
    /// <summary>
    /// Represents a network connection.
    /// </summary>
    public class Client
    {
        private TcpClient client;

        private NetworkStream stream;

        private Thread readThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        /// <param name="client"> The client to use as a connection. </param>
        public Client(TcpClient client)
        {
            this.client = client;
            this.stream = client.GetStream();

            this.readThread = new Thread(this.Read);
            this.readThread.Start();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        /// <param name="ipep"> The remote IPEndPoint to connect to. </param>
        public Client(IPEndPoint ipep)
        {
            this.client = new TcpClient();
            this.client.Connect(ipep);

            this.stream = this.client.GetStream();

            this.readThread = new Thread(this.Read);
            this.readThread.Start();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        /// <param name="ip"> The remote IP to connect to. </param>
        /// <param name="port"> The remote port to connect to. </param>
        public Client(IPAddress ip, int port)
        {
            this.client = new TcpClient();
            this.client.Connect(ip, port);

            this.stream = this.client.GetStream();

            this.readThread = new Thread(this.Read);
            this.readThread.Start();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        /// <param name="hostname"> The remote hostname to connect to. </param>
        /// <param name="port"> The remote port to connect to. </param>
        public Client(string hostname, int port)
        {
            this.client = new TcpClient();
            this.client.Connect(hostname, port);

            this.stream = this.client.GetStream();

            this.readThread = new Thread(this.Read);
            this.readThread.Start();
        }

        /// <summary>
        /// Fires when the connection receives a message.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Fires when the remote becomes disconnected.
        /// </summary>
        public event EventHandler<DisconnectedEventArgs> Disconnected;

        /// <summary>
        /// Writes a message to the specified network stream.
        /// </summary>
        /// <param name="message"> The message to be sent. </param>
        public void Send(string message)
        {
            this.Send(Encoding.ASCII.GetBytes(message));
        }

        /// <summary>
        /// Writes the payload to the specified network stream.
        /// </summary>
        /// <param name="payload"> The payload to be sent. </param>
        public void Send(byte[] payload)
        {
            // Encode message in bytes
            byte[] lenBuffer = BitConverter.GetBytes(payload.Length); // 4 bytes

            // Concat two byte arrays
            byte[] buffer = new byte[4 + payload.Length];
            lenBuffer.CopyTo(buffer, 0);
            payload.CopyTo(buffer, 4);

            // Send the data
            this.stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Disconnects from the remote
        /// </summary>
        public void Disconnect()
        {
            this.OnDisconnected(new DisconnectedEventArgs((this.client.Client.RemoteEndPoint as IPEndPoint).Address));

            this.readThread.Abort();
            this.client.Close();
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
        /// Manages and fires the <see cref="Disconnected" /> event.
        /// </summary>
        /// <param name="e"> The event arguments to fire with. </param>
        protected virtual void OnDisconnected(DisconnectedEventArgs e)
        {
            EventHandler<DisconnectedEventArgs> handler = this.Disconnected;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Reads data from the connection.
        /// </summary>
        /// <remarks>
        /// This is a blocking method. It will return on client disconnect.
        /// </remarks>
        private void Read()
        {
            while (true)
            {
                byte[] message;

                try
                {
                    // Convert the size to an integer
                    int size = this.ReadPayloadSize();

                    message = this.ReadPayload(size);
                }
                catch
                {
                    break;
                }

                if (message.Length == 0)
                {
                    // Client disconnected
                    break;
                }

                // Message received
                string msg = Encoding.ASCII.GetString(message, 0, message.Length);

                this.OnMessageReceived(new MessageReceivedEventArgs(msg, (this.client.Client.RemoteEndPoint as IPEndPoint).Address));
            }

            this.OnDisconnected(new DisconnectedEventArgs((this.client.Client.RemoteEndPoint as IPEndPoint).Address));
        }

        private int ReadPayloadSize()
        {
            // Read the size of the message
            byte[] sizeBuffer = new byte[4];

            // Init message framing variables
            int totalRead = 0, currentRead = 0;

            // Read the size
            do
            {
                currentRead = this.stream.Read(sizeBuffer, totalRead, sizeBuffer.Length - totalRead);

                totalRead += currentRead;
            }
            while (totalRead < sizeBuffer.Length && currentRead > 0);

            // Convert the size to an integer
            return BitConverter.ToInt32(sizeBuffer, 0);
        }

        private byte[] ReadPayload(int size)
        {
            // Assign the payload buffer array
            byte[] message = new byte[size];

            // Asign message framing variables
            int totalRead = 0, currentRead = 0;

            // Read the payload
            do
            {
                currentRead = this.stream.Read(message, totalRead, message.Length - totalRead);

                totalRead += currentRead;
            }
            while (totalRead < message.Length && currentRead > 0);

            return message;
        }
    }
}
