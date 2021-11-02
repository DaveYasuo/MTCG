using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServerModule.Tcp
{
    public class TcpListen
    {
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

        public static async Task<TcpListen> TcpTask()
        {
            TcpListener server = new TcpListener(IPAddress.Loopback, 10001);
            server.Start(5);
            Console.CancelKeyPress += (sender, e) => Environment.Exit(0);

            while (true)
            {
                try
                {
                    //Task<TcpClient> socket = listener.AcceptTcpClientAsync();
                    //Task.WaitAny(socket);
                    //TcpClient server = socket.Result;
                    using TcpClient client = await Task.Run(() => server.AcceptTcpClientAsync());
                    await using StreamWriter writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                    await writer.WriteLineAsync("Welcome to my server!");
                    await using NetworkStream reader = client.GetStream();
                    string message = ToString(reader);
                    Console.WriteLine($"received: {message}");
                    await writer.WriteLineAsync("\nThank you, next! ...");
                }
                catch (Exception exc)
                {
                    Console.WriteLine("error occurred: " + exc.Message);
                }
            }
        }
        public static string ToString(NetworkStream stream)
        {
            using MemoryStream memoryStream = new MemoryStream();
            byte[] data = new byte[1024];
            do
            {
                int size = stream.Read(data, 0, data.Length);
                if (size == 0)
                {
                    Console.WriteLine("client disconnected...");
                    Console.ReadLine();
                    return null;
                }
                memoryStream.Write(data, 0, size);
            } while (stream.DataAvailable);
            return Encoding.ASCII.GetString(memoryStream.ToArray());
        }

    }
}
