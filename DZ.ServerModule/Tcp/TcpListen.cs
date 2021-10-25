using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerModule.Tcp
{
    public class Tcp
    {
        public static async Task TcpTask()
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
                    await writer.WriteLineAsync("Please enter your commands...");

                    using var reader = new StreamReader(socket.GetStream());
                    string message;
                    do
                    {
                        message = await reader.ReadLineAsync();
                        Console.WriteLine("received: " + message);
                    } while (message != "quit");
                }
                catch (Exception exc)
                {
                    Console.WriteLine("error occurred: " + exc.Message);
                }
            }
        }
    }
}
