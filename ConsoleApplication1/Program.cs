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



            byte[] buffer = Encoding.ASCII.GetBytes("yo yo wassup");

            byte[] lenBuffer = BitConverter.GetBytes(buffer.Length);

            Console.WriteLine("Client: Size (bytes): " + buffer.Length);

            clientStream.Write(lenBuffer, 0, 4);
            clientStream.Write(buffer, 0, buffer.Length);

            clientStream.Flush();
        }
    }
}
