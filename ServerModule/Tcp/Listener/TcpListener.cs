using System;
using System.Net;
using System.Net.Sockets;
using ServerModule.Tcp.Client;
using TcpClient = ServerModule.Tcp.Client.TcpClient;

namespace ServerModule.Tcp.Listener
{
    public class TcpListener : ITcpListener
    {
        private readonly System.Net.Sockets.TcpListener _server;

        public TcpListener()
        {
            Console.WriteLine("TcpListener1");
            // default port
            int port = 10001;
            _server = new System.Net.Sockets.TcpListener(IPAddress.Loopback, port);
        }

        public TcpListener(int port)
        {
            Console.WriteLine("TcpListener");
            try
            {
                _server = new System.Net.Sockets.TcpListener(IPAddress.Loopback, port);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        public void Start()
        {
            try
            {
                _server.Start();
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                _server.Stop();
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public ITcpClient AcceptTcpClient()
        {
            return new TcpClient(_server.AcceptTcpClient());
        }
    }
}