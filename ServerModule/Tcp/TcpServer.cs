using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DebugAndTrace;
using ServerModule.SimpleLogic.Handler;
using ServerModule.SimpleLogic.Mapping;
using ServerModule.SimpleLogic.Requests;
using ServerModule.SimpleLogic.Responses;
using ServerModule.Tcp.Client;
using ServerModule.Tcp.Listener;

namespace ServerModule.Tcp
{
    public class TcpServer : IServer
    {
        private bool _listening;
        private readonly IPrinter _printer = Printer.Instance;
        private readonly ITcpListener _server;
        private readonly IMapping _mapping;
        private readonly ConcurrentDictionary<string, Task> _tasks = new();
        private CancellationTokenSource _tokenSource;

        public TcpServer(ITcpListener server, IMapping mapping)
        {
            _server = server;
            _mapping = mapping;
        }

        public TcpServer(ITcpListener server)
        {
            _mapping = new Mapping();
            _server = server;
        }

        public TcpServer()
        {
            _server = new TcpListener(10001);
            _mapping = new Mapping();
        }

        ~TcpServer() => Stop();

        /// <summary>
        /// Starts the server.
        /// For every request a new Task is spawned to work off multiple request at once.
        /// </summary>
        public void Start()
        {
            if (_listening) return;
            _listening = true;
            // Generate cancellation token
            // See: https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-cancel-a-task-and-its-children
            _tokenSource = new CancellationTokenSource();
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
                    ITcpClient client = _server.AcceptTcpClient();
                    //_printer.WriteLine($"Task {id} Waiting for complete");
                    Task task = Task.Run(() => Process(client), token);
                    _tasks[id] = task;
                    _printer.WriteLine($"Task {id} added to the collection");
                    // Remove task from collection when finished
                    // See: https://stackoverflow.com/a/6033036/12347616
                    task.ContinueWith(t =>
                    {
                        _printer.WriteLine($"Task {id} Trying to remove task from collection");
                        if (t == null) return;
                        if (_tasks.TryRemove(id, out t))
                            _printer.WriteLine($"Task {id} Removed task from collection successfully");
                        client.Close();
                    }, token);
                }
                catch (Exception)
                {
                    // Prevent SocketException when break
                    // _printer.WriteLine($"Exc: {exception.Message}");
                }
            }
        }

        /// <summary>
        /// Processes a new client request and returns a response to them.
        /// </summary>
        /// <param name="client"></param>
        private void Process(ITcpClient client)
        {
            //_printer.WriteLine($"Task {Task.CurrentId} Processing");
            if (client == null) return;
            //_printer.WriteLine($"Task {Task.CurrentId} Sleeping");
            // await Task.Delay(5000); 
            //_printer.WriteLine($"Task {Task.CurrentId} Awaken");

            // Here send the message to the ServiceHandler and back to client
            ServiceHandler service = new ServiceHandler(_mapping);
            Request request = client.ReadRequest(_mapping);
            Response response = service.HandleRequest(request);
            client.SendResponse(response);
        }

        /// <summary>
        /// Stops the server.
        /// Disposes all created tasks and closes the socket.
        /// </summary>
        public void Stop()
        {
            if (_listening == false) return;
            // Stop listening
            _listening = false;
            // Cleanup tasks
            _tokenSource.Cancel();
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
                    // Prevent TaskCanceledException
                    // _printer.WriteLine($"Exc: {exception.Message}");
                }
            }

            _tokenSource.Dispose();
            _tasks.Clear();
            _server.Stop();
        }
    }
}
