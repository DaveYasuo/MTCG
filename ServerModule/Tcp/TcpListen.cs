using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic;
using DebugAndTrace;

namespace ServerModule.Tcp
{
    public class TcpListen
    {
        private readonly IPrinter _printer;

        public TcpListen(IPrinter printer)
        {
            _printer = printer;
        }

        private static int BinaryMatch(byte[] input, byte[] pattern)
        {
            int sLen = input.Length - pattern.Length + 1;
            for (int i = 0; i < sLen; ++i)
            {
                bool match = true;
                for (int j = 0; j < pattern.Length; ++j)
                {
                    if (input[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return i;
                }
            }
            return -1;
        }

        public async Task TcpTask()
        {
            TcpListener server = new TcpListener(IPAddress.Loopback, 10001);
            server.Start(5);
            Console.CancelKeyPress += (sender, e) =>
            {
                server.Stop();
                _printer.WriteLine("Server wurde geschlossen.");
                Environment.Exit(0);
            };
            ServiceHandler serviceHandler = new ServiceHandler();
            while (true)
            {
                try
                {
                    using TcpClient client = await server.AcceptTcpClientAsync();
                    //using TcpClient client = await Task.Run(() => server.AcceptTcpClientAsync());
                    await using StreamWriter writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                    await writer.WriteLineAsync("Welcome to my server!");
                    using NetworkStream reader = client.GetStream();
                    string message = ToString(reader);
                    if (message == null)
                    {
                        return;
                    }
                    _printer.WriteLine($"\n\nreceived:\n{message}");
                    //Here send the message to the ServiceHandler
                    serviceHandler.Handle(message);
                    _printer.WriteLine("\nThank you, next! ...");
                    await writer.WriteLineAsync("\nThank you, next! ...");
                }
                catch (Exception exc)
                {
                    _printer.WriteLine("error occurred: " + exc.Message);
                }
            }
        }
        public string ToString(NetworkStream stream)
        {
            using MemoryStream memoryStream = new MemoryStream();
            byte[] data = new byte[1024];
            do
            {
                int size = stream.Read(data, 0, data.Length);
                if (size == 0)
                {
                    _printer.WriteLine("client disconnected...");
                    return null;
                }
                memoryStream.Write(data, 0, size);
            } while (stream.DataAvailable);
            return Encoding.ASCII.GetString(memoryStream.ToArray());
        }

    }
}
