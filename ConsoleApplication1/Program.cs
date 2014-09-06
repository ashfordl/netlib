using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NetLib.Events;
using NetLib.Tcp;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 6565;

            Server server = new Server(port);
            server.MessageReceived += Program.Server_MessageReceived;
            server.ClientDisconnected += Program.Server_ClientDisconnected;
            server.Serve();

            Client client = new Client(IPAddress.Parse("127.0.0.1"), port);

            client.Send("Bonjour, wie gehts dir?");

            Thread.Sleep(2000);

            client.Disconnect();
        }

        static void Server_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine("Server received message: " + e.Message);
        }

        static void Server_ClientDisconnected(object sender, DisconnectedEventArgs e)
        {
            Console.WriteLine("Client " + e.RemoteIP + " disconnected");
        }
    }
}
