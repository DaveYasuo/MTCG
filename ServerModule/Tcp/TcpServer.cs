using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DebugAndTrace;
using ServerModule.Mapping;
using ServerModule.Requests;
using ServerModule.Responses;
using ServerModule.Security;
using ServerModule.Tcp.Client;
using ServerModule.Tcp.Listener;
using ServerModule.Utility;

namespace ServerModule.Tcp
{
    public class TcpServer : IServer
    {
        private bool _listening;
        private readonly IPrinter _log;
        private readonly ITcpListener _server;
        private readonly IMap _map;
        private readonly Authentication _auth;
        private readonly ConcurrentDictionary<string, Task> _tasks = new();
        private CancellationTokenSource _tokenSource;

        public TcpServer(ITcpListener server, IMap map, Authentication auth, IPrinter logger)
        {
            _server = server;
            _map = map;
            _auth = auth;
            _log = logger;
        }

        public TcpServer(int port, IMap map, Authentication auth, IPrinter logger)
        {
            _server = new TcpListener(port);
            _map = map;
            _auth = auth;
            _log = logger;
        }

        public TcpServer(ITcpListener server, IRequestHandler requestHandler, Authentication auth, IPrinter logger)
        {
            _server = server;
            _auth = auth;
            _map = new Map(requestHandler);
            _log = logger;
        }

        ~TcpServer() => Stop();

        /// <summary>
        /// Starts the server. Every request generates a new Task.
        /// </summary>
        public void Start()
        {
            if (_listening) return;
            _listening = true;
            // Generate cancellation token
            // See: https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-cancel-a-task-and-its-children
            _tokenSource = new CancellationTokenSource();
            _server.Start();
            _log.WriteLine("Ready to accept clients");
            Console.CancelKeyPress += (_, e) =>
            {
                Stop();
                _log.WriteLine($"Server closed by: {e.SpecialKey}.");
                Environment.Exit(0);
            };
            // Wait for request and work through them
            // See: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?view=net-5.0#examples
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
                    //_printer.WriteLine($"Task {id} added to the collection");
                    // Remove task from collection when finished
                    // See: https://stackoverflow.com/a/6033036/12347616
                    task.ContinueWith(t =>
                    {
                        //_printer.WriteLine($"Task {id} Trying to remove task from collection");
                        if (t == null) return;
                        _tasks.TryRemove(id, out t);
                        //_printer.WriteLine($"Task {id} Removed task from collection successfully");
                        client.Close();
                    }, token);
                }
                catch (Exception e)
                {
                    // Prevent SocketException when break
                    _log.WriteLine(e.Message);
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

            Request request = client.ReadRequest(in _map);
            Response response = HandleRequest(in request);
            client.SendResponse(in response);
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
            //_printer.WriteLine($"Number of tasks: {_tasks.Count}");
            foreach (var task in _tasks.Values)
            {
                if (task.IsCompleted) continue;
                try
                {
                    //_printer.WriteLine($"Wait for Task {task.Id}");
                    _log.WriteLine(task.Wait(500) ? "Task completed" : "Task failed.");
                }
                catch (Exception e)
                {
                    // Prevent TaskCanceledException
                    _log.WriteLine(e.Message);
                }
            }

            _tokenSource.Dispose();
            _tasks.Clear();
            _server.Stop();
        }

        public Response HandleRequest(in Request request)
        {
            // See: https://developer.mozilla.org/de/docs/Web/HTTP/Status

            // if Request has unsupported syntax
            if (request == null) return Response.Status(Status.BadRequest);
            // if HTTP Version is unsupported
            if (request.Version != "HTTP/1.1") return Response.Status(Status.HttpVersionNotSupported);
            // Check if method is supported
            switch (request.Method)
            {
                // if Method is not allowed the server must generate an Allow header field in a 405 status code response.
                // The field must contain a list of methods that the target resource currently supports.
                // See: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Allow
                case Method.Patch or Method.Head:
                    {
                        Dictionary<string, string> allowedHeaders = new() { { "Allow", "GET, POST, PUT, DELETE" } };
                        return Response.Status(Status.MethodNotAllowed, allowedHeaders);
                    }
                case Method.Error:
                    return Response.Status(Status.NotImplemented);
            }
            // if path is unsupported
            if (!_map.Contains(request.Method, request.Target)) return Response.Status(Status.NotFound);
            // Proceed handling Request:
            // Check if path is secured, if not just invoke the corresponding function
            if (_auth == null || !_auth.PathIsSecured(request.Method, request.Target)) return _map.InvokeMethod(request.Method, request.Target, null, request.Payload, request.PathVariable, request.RequestParam);
            // Check if Authorization header was send
            if (!request.Headers.ContainsKey("Authorization")) return Response.Status(Status.Unauthorized);
            // If so, check credentials
            string[] line = request.Headers["Authorization"].Split(Utils.GetChar(Utility.Char.WhiteSpace));
            if (line.Length != 2) return Response.Status(Status.Forbidden);
            string type = line[0];
            string token = line[1];
            // Check if Authentication is valid
            if (!_auth.Check(type, token)) return Response.Status(Status.Forbidden);
            // Get Token Details
            AuthToken authToken = _auth.GetDetails(token);
            // invoke the corresponding function
            return _map.InvokeMethod(request.Method, request.Target, authToken, request.Payload, request.PathVariable, request.RequestParam);
        }
    }
}
