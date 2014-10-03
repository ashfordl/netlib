using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NetLib;
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

            server.MessageReceived += Program.server_MessageReceived;
            server.ClientDisconnected += Program.server_ClientDisconnected;
            server.ClientConnected += Program.server_ClientConnected;

            server.Serve();

            Client client = new Client(IPAddress.Parse("127.0.0.1"), port);
            client.MessageReceived += Program.client_MessageReceived;

            client.Send(new Message("Bonjour, wie gehts dir?"));

            Thread.Sleep(250);

            server.MessageAllClients(new Message("Broadcast"));

            Thread.Sleep(250);

            client.Disconnect();
        }

        private static void server_ClientConnected(object sender, ConnectedEventArgs e)
        {
            Console.WriteLine("Client connected from IP  " + e.RemoteIP);
        }

        static void server_MessageReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("Server received message: " + e.Message);
        }

        static void server_ClientDisconnected(object sender, DisconnectedEventArgs e)
        {
            Console.WriteLine("Client disconnected from IP " + e.RemoteIP);
        }

        static void client_MessageReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("Client received message: " + e.Message);
        }
    }
}
