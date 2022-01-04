using ServerModule.SimpleLogic.Mapping;
using ServerModule.SimpleLogic.Requests;
using ServerModule.SimpleLogic.Responses;

namespace ServerModule.Tcp.Client
{
    public interface ITcpClient
    {
        Request ReadRequest(IMapping mapping);
        void SendResponse(Response response);
        public void Close();
    }
}