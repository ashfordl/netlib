// Client.cs
// <copyright file="Client.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetLib.Client
{
    /// <summary>
    /// Represents a connection to a <see cref="NetLib.Server.Server" />.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// The TCP connection object.
        /// </summary>
        private TcpClient tcp;

        /// <summary>
        /// The underlying stream of data.
        /// </summary>
        private NetworkStream stream;

        /// <summary>
        /// The thread used to read incoming data.
        /// </summary>
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

        /// <summary>
        /// Sends the message to the server.
        /// </summary>
        /// <param name="message"> The message to be sent. </param>
        public void Send(string message)
        {
            this.Send(Encoding.ASCII.GetBytes(message));
        }

        /// <summary>
        /// Sends the payload to the server.
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

            Console.WriteLine("Client: Message sent");
        }

        /// <summary>
        /// Initializes and starts the read thread.
        /// </summary>
        private void InitReadThread()
        {
            this.readThread = new Thread(this.Read);
            this.readThread.Start();
        }

        /// <summary>
        /// Reads the size, in bytes, of the message payload.
        /// </summary>
        /// <returns> The number of bytes of the payload. </returns>
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

        /// <summary>
        /// Reads the message payload.
        /// </summary>
        /// <param name="size"> The size, in bytes, of the payload. </param>
        /// <returns> The payload. </returns>
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

        /// <summary>
        /// Reads data from the connection.
        /// </summary>
        /// <remarks>
        /// This is a blocking method.
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
                Console.WriteLine("Client: " + msg);
            }

            this.tcp.Close();
        }
    }
}
