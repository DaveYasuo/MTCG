using System.Net;
using ServerModule.Tcp.Client;
using TcpClient = ServerModule.Tcp.Client.TcpClient;

namespace ServerModule.Tcp.Listener
{
    /// <summary>
    /// Wrapper Class for the TcpListener
    /// </summary>
    /// <inheritdoc cref="ITcpListener"/>
    public class TcpListener : ITcpListener
    {
        private readonly System.Net.Sockets.TcpListener _server;

        public TcpListener(int port)
        {
            _server = new System.Net.Sockets.TcpListener(IPAddress.Loopback, port);
        }

        public void Start()
        {
            _server.Start();
        }

        public void Stop()
        {
            _server.Stop();
        }

        public ITcpClient AcceptTcpClient()
        {
            return new TcpClient(_server.AcceptTcpClient());
        }
    }
}