using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BusinessLogic;
using DatabaseModule;
using DebugAndTrace;

namespace ServerModule.Tcp
{
    public class TcpServer : IServer
    {
        private readonly IPrinter _printer;
        private readonly int _port = 10001;
        private readonly TcpListener _server;
        private bool _listening = true;
        private readonly ConcurrentDictionary<string, Task> _tasks = new();
        // Generate cancellation token
        // See: https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-cancel-a-task-and-its-children
        private readonly CancellationTokenSource _tokenSource = new();

        public TcpServer(IPrinter printer)
        {
            _printer = printer;
            _server = new TcpListener(IPAddress.Loopback, _port);
        }

        public TcpServer(IPrinter printer, int port)
        {
            _port = port;
            _printer = printer;
            _server = new TcpListener(IPAddress.Loopback, _port);
        }

        /// <summary>
        /// Starts the server.
        /// For every request a new Task is spawned to work off multiple request at once.
        /// </summary>
        public void Start()
        {
            _server.Start();
            Console.CancelKeyPress += (_, e) =>
            {
                Stop();
                _printer.WriteLine($"Server closed by: {e.SpecialKey}.");
                Environment.Exit(0);
            };
            // Wait for request and work through them
            // See: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?view=net-5.0#see-also
            //int x = 0;
            while (_listening)
            {
                try
                {
                    //_printer.WriteLine($"Number: {++x}");
                    // Get token
                    CancellationToken token = _tokenSource.Token;
                    // Generate GUID
                    // See: https://stackoverflow.com/a/4421513/12347616
                    string id = Guid.NewGuid().ToString();
                    TcpClient client = _server.AcceptTcpClient();
                    //_printer.WriteLine($"Task {id} Waiting for complete");
                    Task task = Task.Run(async () => await Process(client), token);
                    _tasks[id] = task;
                    // Remove task from collection when finished
                    // See: https://stackoverflow.com/a/6033036/12347616
                    task.ContinueWith(t =>
                    {
                        _printer.WriteLine($"Task {id} Trying to remove task from collection");
                        if (t == null) return;
                        if (_tasks.TryRemove(id, out t)) _printer.WriteLine($"Task {id} Removed task from collection successfully");
                    }, token);
                }
                catch (Exception)
                {
                    // ignored
                    // Prevent SocketException when break
                    // _printer.WriteLine($"Exc: {exception.Message}");
                }
            }
        }

        /// <summary>
        /// Stops the server.
        /// Disposes all created tasks and closes the socket.
        /// </summary>
        public void Stop()
        {
            // Stop listening
            _listening = false;
            // Cleanup tasks
            _tokenSource.Cancel();
            if (_tokenSource.IsCancellationRequested)
            {
                _printer.WriteLine("Canceled Token");
            }
            _printer.WriteLine($"Number of tasks: {_tasks.Count}");
            foreach (var task in _tasks.Values)
            {
                if (task.IsCompleted) continue;
                try
                {
                    _printer.WriteLine($"Wait for Task {task.Id}");
                    _printer.WriteLine(task.Wait(500) ? "Task is complete" : "Task failed.");
                }
                catch (Exception)
                {
                    // ignored
                    // Prevent TaskCanceledException
                    // _printer.WriteLine($"Exc: {exception.Message}");
                }
            }

            _tasks.Clear();
            // Stop listener
            _server.Stop();
        }
        /// <summary>
        /// Processes a new client request and returns a response to them.
        /// </summary>
        /// <param name="client"></param>
        private async Task Process(TcpClient client)
        {
            //_printer.WriteLine($"Task {Task.CurrentId} Processing");
            if (client == null) return;
            //_printer.WriteLine($"Task {Task.CurrentId} Sleeping");
            // await Task.Delay(5000); 
            //_printer.WriteLine($"Task {Task.CurrentId} Awaken");
            try
            {
                await using StreamWriter writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
                await writer.WriteLineAsync("Welcome to my server!");
                await using NetworkStream reader = client.GetStream();
                string message = ToString(reader);
                if (message == null) return;
                _printer.WriteLine($"\n\nreceived:\n{message}");
                // Here send the message to the ServiceHandler and back to client
                await writer.WriteLineAsync(new ServiceHandler().Handle(message));
            }
            catch (Exception exc)
            {
                _printer.WriteLine("error occurred: " + exc.Message);
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
