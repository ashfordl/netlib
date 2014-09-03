// Client.cs
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetLib.Client
{
    public class Client
    {
        private TcpClient tcp;
        private NetworkStream stream;

        private Thread readThread;

        public Client(IPEndPoint ipep)
        {
            this.tcp = new TcpClient();
            this.tcp.Connect(ipep);

            this.stream = this.tcp.GetStream();

            this.InitReadThread();
        }

        public Client(IPAddress ip, int port)
        {
            this.tcp = new TcpClient();
            this.tcp.Connect(new IPEndPoint(ip, port));

            this.stream = this.tcp.GetStream();

            this.InitReadThread();
        }

        public Client(string hostname, int port)
        {
            this.tcp = new TcpClient(hostname, port);
            this.stream = this.tcp.GetStream();

            this.InitReadThread();
        }

        private void InitReadThread()
        {
            this.readThread = new Thread(this.Read);
            readThread.Start();
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
                Console.WriteLine("Client: " + msg);
            }

            this.tcp.Close();
        }

        public void Send(string message)
        {
            this.Send(Encoding.ASCII.GetBytes(message));
        }

        public void Send(byte[] message)
        {
            // Encode message in bytes
            byte[] lenBuffer = BitConverter.GetBytes(message.Length); // 4 bytes

            // Concat two byte arrays
            byte[] buffer = new byte[4 + message.Length];
            lenBuffer.CopyTo(buffer, 0);
            message.CopyTo(buffer, 4);

            // Send the data
            this.stream.Write(buffer, 0, buffer.Length);

            Console.WriteLine("Client: Message sent");
        }
    }
}
