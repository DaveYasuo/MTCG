using System.Collections.Generic;
using MTCG.BattleLogic;
using MTCG.Data.Users;

namespace MTCG.Logging
{
    public class BattleLog : IBattleLog
    {
        private readonly IPlayer _player1;
        private readonly IPlayer _player2;
        public List<object> LogList { get; }
        private string _winner;

        public BattleLog(IPlayer player1, IPlayer player2)
        {
            _player1 = player1;
            _player2 = player2;
            LogList = new List<object>();
        }

        public void AddRound(float damage1, float damage2, int round)
        {
            _winner = damage1.CompareTo(damage2) switch
            {
                > 0 => _player1.Username,
                < 0 => _player2.Username,
                _ => null
            };
            RoundLog roundLog = new RoundLog
            {
                Round = round,
                Title = $"{_player1.LastPlayedCard}: {damage1} VS {_player2.LastPlayedCard}: {damage2}",
                RemainingCards = $" {_player1.Cards.Count} VS {_player2.Cards.Count}",
                Result = _winner is { } ? $"{_winner} wins the round" : "Draw"
            };
            Add(roundLog);
            if (_player1.Cards.Count == 0 || _player2.Cards.Count == 0) AddResult();
            if (round != 100) return;
            _winner = null;
            AddResult();
        }

        private void AddResult()
        {
            Add(_winner == null ? "Draw Game" : $"Player {_winner} wins the game");
            _player1.Log.AddRange(LogList);
            _player2.Log.AddRange(LogList);
        }

        /// <summary>
        /// Wrapper for adding to the Log
        /// </summary>
        /// <param name="message"></param>
        private void Add(object message) => LogList.Add(message);

        public BattleResult GetResult() => _winner == null ? new BattleResult(LogList) : new BattleResult(_winner, _winner == _player1.Username ? _player2.Username : _player1.Username, LogList);

        public void AddStartInfo() => Add($"Player {_player1.Username} VS Player {_player2.Username}");
    }
}