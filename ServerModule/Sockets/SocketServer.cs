using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerModule.Sockets
{
    class SocketServer
    {
        private static void StartServer()
        {
            // Get Host IP Address that is used to establish a connection  
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
            // If a host has multiple addresses, you will get a list of addresses  
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 10001);
            try
            {
                // Create a Socket that will use Tcp protocol   
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // A Socket must be associated with an endpoint using the Bind method 
                listener.Bind(localEndPoint);
                // Specify how many requests a Socket can listen before it gives Server busy response.  
                // We will listen 10 requests at a time  
                listener.Listen();

                Console.WriteLine("Waiting for a connection...");
                Socket handler = listener.Accept();

                // Incoming data from the client.  
                MemoryStream memoryStream = new MemoryStream();
                string data = null;
                var bufferBytes = new byte[1024];
                int bytesRec = handler.Receive(bufferBytes);

                while (bytesRec > 0)
                {
                    memoryStream.Write(bufferBytes, 0, bytesRec);
                    if (listener.Available > 0)
                    {
                        bytesRec = handler.Receive(bufferBytes);
                    }
                    else
                    {
                        break;
                    }
                }

                byte[] totalBytes = memoryStream.ToArray();
                memoryStream.Close();

                data = Encoding.ASCII.GetString(totalBytes);
                Console.WriteLine("Text received : \n{0}", data);

                byte[] msg = Encoding.ASCII.GetBytes(data);
                handler.Send(msg);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
