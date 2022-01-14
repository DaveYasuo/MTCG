using ServerModule.Mapping;
using ServerModule.Requests;
using ServerModule.Responses;

namespace ServerModule.Tcp.Client
{
    public interface ITcpClient
    {
        Request ReadRequest();
        void SendResponse(in Response response);
        public void Close();
    }
}