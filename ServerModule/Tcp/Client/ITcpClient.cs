using ServerModule.Mapping;
using ServerModule.Requests;
using ServerModule.Responses;

namespace ServerModule.Tcp.Client
{
    public interface ITcpClient
    {
        Request ReadRequest(in IMap map);
        void SendResponse(in Response response);
        public void Close();
    }
}