﻿// Connection.cs
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetLib.Server
{
    class Connection
    {
        private TcpClient client;
        private NetworkStream stream;

        public Connection(TcpClient client)
        {
            this.client = client;
            this.stream = client.GetStream();

            Console.WriteLine("Server: Client loaded");

            this.Read();
        }

        protected int ReadPayloadSize()
        {
            // Read the size of the message
            byte[] sizeBuffer = new byte[4];

            // Init message framing variables
            int totalRead = 0, currentRead = 0;

            // Initial read
            currentRead = totalRead = this.stream.Read(sizeBuffer, 0, 4);

            // Ensure that all 4 bytes have been read
            while (totalRead < sizeBuffer.Length && currentRead > 0)
            {
                currentRead = this.stream.Read(sizeBuffer, totalRead, sizeBuffer.Length - totalRead);

                totalRead += currentRead;
            }

            // Convert the size to an integer
            return BitConverter.ToInt32(sizeBuffer, 0);
        }

        protected byte[] ReadPayload(int size)
        {
            // Assign the payload buffer array
            byte[] message = new byte[size];

            // Asign message framing variables
            int totalRead = 0, currentRead = 0;

            // Read the payload
            currentRead = totalRead = this.stream.Read(message, 0, size);

            // Ensure that the entire payload is read
            while (totalRead < message.Length && currentRead > 0)
            {
                currentRead = this.stream.Read(message, totalRead, message.Length - totalRead);

                totalRead += currentRead;
            }

            return message;
        }

        protected void Read()
        {
            while (true)
            {
                byte[] message;

                try
                {
                    // Convert the size to an integer
                    int size = this.ReadPayloadSize();

                    Console.WriteLine("Server: Size (bytes): " + size);

                   
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
                System.Diagnostics.Debug.WriteLine("Server: "+msg);
            }

            this.client.Close();
        }

        public void Send(string message)
        {
            // Encode message in bytes
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] buffer = encoder.GetBytes(message);

            this.stream.Write(buffer, 0, buffer.Length);
            this.stream.Flush();
        }
    }
}
