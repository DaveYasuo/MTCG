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
            TcpListener listener = new TcpListener(IPAddress.Loopback, 10001);
            listener.Start(5);

            Console.CancelKeyPress += (sender, e) => Environment.Exit(0);

            while (true)
            {
                try
                {
                    var socket = await listener.AcceptTcpClientAsync();
                    await using var writer = new StreamWriter(socket.GetStream()) { AutoFlush = true };
                    await writer.WriteLineAsync("Welcome to my server!");
                    byte[] data = new byte[1024];
                    string message;
                    do
                    {
                        await using NetworkStream reader = socket.GetStream();
                        Console.WriteLine("received1: " + ToString(reader));
                        reader.Read(data);
                        message = Encoding.ASCII.GetString(data);
                        Console.WriteLine("received1: " + message);
                        await writer.WriteLineAsync("Thank you, next! ...");
                    } while (message != "quit");
                }
                catch (Exception exc)
                {
                    Console.WriteLine("error occurred: " + exc.Message);
                }
            }
        }
        public static string ToString(NetworkStream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            byte[] data = new byte[256];
            int size;
            do
            {
                size = stream.Read(data, 0, data.Length);
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
