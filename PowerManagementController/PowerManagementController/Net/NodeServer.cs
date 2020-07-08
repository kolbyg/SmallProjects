using System;
using System.Collections.Generic;
using System.Text;
using NetCoreServer;
using System.Net;
using System.Net.Sockets;

namespace PowerManagementController.Net
{
    class NodeServer : IDisposable
    {
        NodeCommServer server;
        public NodeServer(int ListenPort)
        {
            Console.WriteLine($"Starting TCP server on port: {ListenPort}");

            Console.WriteLine();

            // Create a new TCP chat server
            server = new NodeCommServer(IPAddress.Any, ListenPort);

            // Start the server
            Console.Write("Server starting...");
            server.Start();
            Console.WriteLine("Done!");
        }
        public void SendDataToAllClients(string Data)
        {
            if (string.IsNullOrEmpty(Data))
                return;
                server.Multicast(Data);
            }
        public void Restart()
        {
            Console.Write("Server restarting...");
            server.Restart();
            Console.WriteLine("Done!");
        }
        public void Dispose()
        {
            // Stop the server
            Console.Write("Server stopping...");
            server.Stop();
            Console.WriteLine("Done!");
        }
    }
    class NodeSession : TcpSession
    {
        public NodeSession(TcpServer server) : base(server) { }

        protected override void OnConnected()
        {
            Console.WriteLine($"Chat TCP session with Id {Id} connected!");

            // Send invite message
            string message = "Hello from TCP chat! Please send a message or '!' to disconnect the client!";
            SendAsync(message);
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"Chat TCP session with Id {Id} disconnected!");
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
            Console.WriteLine("Incoming: " + message);

            // Multicast message to all connected sessions
            Server.Multicast(message);

            // If the buffer starts with '!' the disconnect the current session
            if (message == "!")
                Disconnect();
        }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP session caught an error with code {error}");
        }
    }

    class NodeCommServer : TcpServer
    {
        public NodeCommServer(IPAddress address, int port) : base(address, port) { }

        protected override TcpSession CreateSession() { return new NodeSession(this); }

        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Chat TCP server caught an error with code {error}");
        }
    }

}
