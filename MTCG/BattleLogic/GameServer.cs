using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DebugAndTrace;
using MTCG.Data.Users;
using MTCG.Models;
using ServerModule;

namespace MTCG.BattleLogic
{
    public class GameServer : IServer
    {
        private readonly IPrinter _log;
        private bool _running;
        private CancellationTokenSource _tokenSource;
        private readonly ConcurrentDictionary<string, Task> _tasks = new();
        private readonly ConcurrentQueue<IPlayer> _queue = new();
        private Thread _serverThread;

        public GameServer(IPrinter log)
        {
            _log = log;
            Start();
        }

        ~GameServer() => Stop();

        public void Start()
        {
            if (_running) return;
            _running = true;
            _tokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                _log.WriteLine($"Game Server closed by: {e.SpecialKey}.");
                Stop();
            };
            _serverThread = new Thread(ServiceHandler);
            _serverThread.Start();
        }

        private void ServiceHandler()
        {
            while (_running)
            {
                try
                {
                    if (_queue.Count < 2)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    // Get players from queue
                    IPlayer player1 = null, player2 = null;
                    // if tryDequeue returns false, out playerA/B's value is default -> null
                    while (player1 == null) _queue.TryDequeue(out player1);
                    while (player2 == null) _queue.TryDequeue(out player2);

                    CancellationToken token = _tokenSource.Token;
                    string id = Guid.NewGuid().ToString();
                    Task task = Task.Run(() => Game(player1, player2), token);
                    _tasks[id] = task;
                    // Remove task from collection when finished
                    task.ContinueWith(delegate (Task t)
                    {
                        if (t == null) return;
                        _tasks.TryRemove(id, out t);
                    }, token);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private void Game(IPlayer player1, IPlayer player2)
        {

        }

        public void Stop()
        {
            if (_running == false) return;
            // Stop listening
            _running = false;
            // Cleanup tasks
            _tokenSource.Cancel();
            foreach (var task in _tasks.Values)
            {
                if (task.IsCompleted) continue;
                try
                {
                    task.Wait(500);
                }
                catch (Exception)
                {
                    // Prevent TaskCanceledException
                }
            }
            _tokenSource.Dispose();
            _tasks.Clear();
            _serverThread.Join();
        }

        private void QueuePlayer(IPlayer player)
        {
            _queue.Enqueue(player);
        }

        public string Play(string username, List<Card> cards)
        {
            IPlayer player = new Player(username, cards);
            QueuePlayer(player);
            return GetResult(player);
        }

        private string GetResult(IPlayer username)
        {
            return "";
        }
    }
}