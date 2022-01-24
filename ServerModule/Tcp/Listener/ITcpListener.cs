using ServerModule.Tcp.Client;

namespace ServerModule.Tcp.Listener
{
    public interface ITcpListener : IServer
    {
        // Generic methods for adding instances to the container
        // See: https://stackoverflow.com/a/15717047
        /// <summary>
        ///     Accepting a pending connection requests
        /// </summary>
        /// <returns>A TcpClient used to send and receive data.</returns>
        ITcpClient AcceptTcpClient();
    }
}