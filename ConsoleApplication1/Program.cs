using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetLib.Client;
using NetLib.Server;
using System.Net.Sockets;
using System.Net;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 6565;

            Server server = new Server(port);
            server.Serve();

            Client(port);
            Console.WriteLine("Client: Message sent");
        }

        static void Client(int port)
        {
            TcpClient client = new TcpClient();

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            client.Connect(serverEndPoint);

            NetworkStream clientStream = client.GetStream();

            string message = "yo hello team";

            // Encode message in bytes
            byte[] msgBuffer = Encoding.ASCII.GetBytes(message);
            byte[] lenBuffer = BitConverter.GetBytes(msgBuffer.Length); // 4 bytes

            // Concat two byte arrays
            byte[] buffer = new byte[4 + msgBuffer.Length];
            lenBuffer.CopyTo(buffer, 0);
            msgBuffer.CopyTo(buffer, 4);

            // Send the data
            clientStream.Write(buffer, 0, buffer.Length);
        }
    }
}
