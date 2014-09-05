using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetLib.Tcp
{
    internal static class StreamUtil
    {
        /// <summary>
        /// Writes a message to the specified network stream.
        /// </summary>
        /// <param name="stream"> The network stream to write to. </param>
        /// <param name="message"> The message to be sent. </param>
        public static void Send(NetworkStream stream, string message)
        {
            StreamUtil.Send(stream, Encoding.ASCII.GetBytes(message));
        }

        /// <summary>
        /// Writes the payload to the specified network stream.
        /// </summary>
        /// <param name="stream"> The network stream to write to. </param>
        /// <param name="payload"> The payload to be sent. </param>
        public static void Send(NetworkStream stream, byte[] payload)
        {
            // Encode message in bytes
            byte[] lenBuffer = BitConverter.GetBytes(payload.Length); // 4 bytes

            // Concat two byte arrays
            byte[] buffer = new byte[4 + payload.Length];
            lenBuffer.CopyTo(buffer, 0);
            payload.CopyTo(buffer, 4);

            // Send the data
            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Reads the size, in bytes, of the message payload.
        /// </summary>
        /// <returns> The number of bytes of the payload. </returns>
        private static int ReadPayloadSize(NetworkStream stream)
        {
            // Read the size of the message
            byte[] sizeBuffer = new byte[4];

            // Init message framing variables
            int totalRead = 0, currentRead = 0;

            // Read the size
            do
            {
                currentRead = stream.Read(sizeBuffer, totalRead, sizeBuffer.Length - totalRead);

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
        private static byte[] ReadPayload(NetworkStream stream, int size)
        {
            // Assign the payload buffer array
            byte[] message = new byte[size];

            // Asign message framing variables
            int totalRead = 0, currentRead = 0;

            // Read the payload
            do
            {
                currentRead = stream.Read(message, totalRead, message.Length - totalRead);

                totalRead += currentRead;
            }
            while (totalRead < message.Length && currentRead > 0);

            return message;
        }

        /// <summary>
        /// Reads data from the connection.
        /// </summary>
        /// <remarks>
        /// This is a blocking method. IT will finish on client disconnect.
        /// </remarks>
        public static void Read(NetworkStream stream, Action<string> messageCallback)
        {
            while (true)
            {
                byte[] message;

                try
                {
                    // Convert the size to an integer
                    int size = StreamUtil.ReadPayloadSize(stream);

                    message = StreamUtil.ReadPayload(stream, size);
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

                messageCallback(msg);
            }
        }
    }
}
