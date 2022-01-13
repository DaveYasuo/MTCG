using ServerModule.Tcp.Client;

namespace ServerModule.Tcp.Listener
{
    public interface ITcpListener : IServer
    {
        /// <summary>
        /// Accepting a pending connection requests
        /// </summary>
        /// <returns>A TcpClient used to send and receive data.</returns>
        ITcpClient AcceptTcpClient();
    }
}