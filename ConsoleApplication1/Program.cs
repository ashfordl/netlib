using NetLib.Client;
using NetLib.Server;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 6565;

            Server server = new Server(port);
            server.Serve();

            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            //Client client = new Client(ip);
            Client client = new Client(IPAddress.Parse("127.0.0.1"), port);
            //Client client = new Client("127.0.0.1", port);

            client.Send("Bonjour, wie gehts dir?");
        }
    }
}
