using DatabaseModule;
using ServerModule.Tcp.Client;

namespace ServerModule.Tcp.Listener
{
    public interface ITcpListener : IServer
    {
        ITcpClient AcceptTcpClient();
    }
}