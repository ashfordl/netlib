using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetLib.Events;
using System.Net;
using System.Threading;

namespace NetLib.Tcp
{
    public class Client
    {
        private TcpClient client;

        private NetworkStream stream;

        private Thread readThread;

        public Client(TcpClient client)
        {
            this.client = client;
            this.stream = client.GetStream();

            this.readThread = new Thread(this.Read);
            this.readThread.Start();
        }

        public Client(IPEndPoint ipep)
        {
            this.client = new TcpClient();
            this.client.Connect(ipep);

            this.stream = client.GetStream();

            this.readThread = new Thread(this.Read);
            this.readThread.Start();
        }

        public Client(IPAddress ip, int port)
        {
            this.client = new TcpClient();
            this.client.Connect(ip, port);

            this.stream = client.GetStream();

            this.readThread = new Thread(this.Read);
            this.readThread.Start();
        }

        public Client(string hostname, int port)
        {
            this.client = new TcpClient();
            this.client.Connect(hostname, port);

            this.stream = client.GetStream();

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
        public event EventHandler<RemoteDisconnectedEventArgs> RemoteDisconnected;

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
            this.OnRemoteDisconnected(new RemoteDisconnectedEventArgs((this.client.Client.RemoteEndPoint as IPEndPoint).Address));

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
        /// Manages and fires the <see cref="RemoteDisconnected" /> event.
        /// </summary>
        /// <param name="e"> The event arguments to fire with. </param>
        protected virtual void OnRemoteDisconnected(RemoteDisconnectedEventArgs e)
        {
            EventHandler<RemoteDisconnectedEventArgs> handler = this.RemoteDisconnected;

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

            this.OnRemoteDisconnected(new RemoteDisconnectedEventArgs((this.client.Client.RemoteEndPoint as IPEndPoint).Address));
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
