using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BusinessLogic;
using DebugAndTrace;
using JetBrains.Annotations;

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

        public async Task StartServer()
        {
            // just some useful information to know how many clients had requested
            int numThreads = 0;
            TcpListener server = new TcpListener(IPAddress.Loopback, 10001);
            server.Start(5);
            // using Countdown to wait for all Threads to finish
            using CountdownEvent finishedEvent = new CountdownEvent(1);
            Console.CancelKeyPress += (sender, e) =>
            {
                // ReSharper disable once AccessToDisposedClosure
                if (!finishedEvent.Signal())
                {
                    // ReSharper disable once AccessToDisposedClosure
                    _printer.WriteLine($"Open processes are: {finishedEvent.CurrentCount}.");
                    const int timeOut = 5000;
                    _printer.WriteLine($"Waiting for {timeOut} milliseconds before shutdown ...");
                    // ReSharper disable once AccessToDisposedClosure
                    _printer.WriteLine(!finishedEvent.Wait(timeOut)
                        // ReSharper disable once AccessToDisposedClosure
                        ? $"Waited for pending unreleased ({finishedEvent.CurrentCount}) WorkItems before closing session but they did not complete in time"
                        : "All WorkItems have been finished.");
                }
                else
                {
                    _printer.WriteLine("All WorkItems have been finished.");
                }
                server.Stop();
                _printer.WriteLine($"Total clients connected: {numThreads}.");
                _printer.WriteLine($"Server closed by: {e.SpecialKey}.");
                Environment.Exit(0);
            };
            while (true)
            {
                try
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    // if you want live updates of the function, you can use delegate, else use x =>
                    finishedEvent.AddCount();
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        try
                        {
                            async void Action() => await ThreadProc(client);
                            Console.WriteLine("Before running");
                            RunThisAction(Action);
                            Console.WriteLine("I'm still running");
                            Interlocked.Increment(ref numThreads);
                        }
                        finally
                        {
                            Console.WriteLine("Done running");
                            // ReSharper disable once AccessToDisposedClosure
                            finishedEvent.Signal();
                        }
                    }, null);
                    Console.WriteLine($"Pending Work Item: {ThreadPool.PendingWorkItemCount}.");

                }
                catch (Exception exc)
                {
                    _printer.WriteLine("Error occurred in accepting clients: " + exc.Message);
                    break;
                }
            }
            finishedEvent.Signal();
            finishedEvent.Wait();
        }

        private static void RunThisAction(Action action)
        {
            action();
        }

        private async Task ThreadProc(object obj)
        {
            Console.WriteLine("Sleeping now");
            using TcpClient client = obj as TcpClient;
            if (client == null)
            {
                return;
            }
            ServiceHandler serviceHandler = new ServiceHandler();
            {
                try
                {
                    //using TcpClient client = await Task.Run(() => server.AcceptTcpClientAsync());
                    await using StreamWriter writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                    await writer.WriteLineAsync("Welcome to my server!");
                    await using NetworkStream reader = client.GetStream();
                    string message = ToString(reader);
                    if (message == null)
                    {
                        return;
                    }
                    _printer.WriteLine($"\n\nreceived:\n{message}");
                    //Here send the message to the ServiceHandler and back to client
                    await writer.WriteLineAsync(serviceHandler.Handle(message));
                }
                catch (Exception exc)
                {
                    _printer.WriteLine("error occurred: " + exc.Message);
                }
            }
            Console.WriteLine("Woke up and finished");
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
