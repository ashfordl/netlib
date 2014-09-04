// Connection.cs
// <copyright file="Connection.cs"> This code is protected under the MIT License. </copyright>
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NetLib.Events;

namespace NetLib.Server
{
    class Connection
    {
        private TcpClient client;
        private NetworkStream stream;

        private Thread readThread;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public Connection(TcpClient client)
        {
            this.client = client;
            this.stream = client.GetStream();

            this.readThread = new Thread(this.Read);
            this.readThread.Start();

            Console.WriteLine("Server: Client loaded");
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            EventHandler<MessageReceivedEventArgs> handler = this.MessageReceived;

            if (handler != null)
            {
                handler(this, e);
            }
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

        private void Read()
        {
            while (true)
            {
                byte[] message;

                try
                {
                    // Convert the size to an integer
                    int size = this.ReadPayloadSize();

                    message = ReadPayload(size);
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

            this.client.Close();
        }

        public void Send(string message)
        {
            // Encode message in bytes
            byte[] msgBuffer = Encoding.ASCII.GetBytes(message);
            byte[] lenBuffer = BitConverter.GetBytes(msgBuffer.Length); // 4 bytes

            // Concat two byte arrays
            byte[] buffer = new byte[4 + msgBuffer.Length];
            lenBuffer.CopyTo(buffer, 0);
            msgBuffer.CopyTo(buffer, 4);

            // Send the data
            this.stream.Write(buffer, 0, buffer.Length);
        }
    }
}
