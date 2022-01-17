using System;
using System.Collections.Generic;
using DebugAndTrace;
using MTCG.BattleLogic;
using MTCG.Data.Users;

namespace MTCG.Logging
{
    public class BattleLog : IBattleLog
    {
        private readonly IPlayer _player1;
        private readonly IPlayer _player2;
        public List<string> LogList { get; }
        private string _winner;

        public BattleLog(IPlayer player1, IPlayer player2)
        {
            _player1 = player1;
            _player2 = player2;
            LogList = new List<string>();
        }

        public void AddRound(float damage1, float damage2, int round)
        {
            Add($"Round {round}: ");
            Add($"{_player1.LastPlayedCard}: Damage {damage1} VS {_player2.LastPlayedCard}: Damage {damage2}");
            switch (damage1.CompareTo(damage2))
            {
                case > 0:
                    Add($"Result: {_player1.Username} wins the round");
                    _winner = _player1.Username;
                    break;
                case < 0:
                    Add($"Result: {_player2.Username} wins the round");
                    _winner = _player2.Username;
                    break;
                default:
                    Add("Result: Draw");
                    _winner = null;
                    break;
            }
            Add($"Remaining cards: {_player1.Cards.Count} VS {_player2.Cards.Count}");
            if (_player1.Cards.Count == 0 || _player2.Cards.Count == 0 || round == 100) AddResult();
        }

        private void AddResult()
        {
            Add("Game Over");
            if (_winner == null)
            {
                Add("Draw Game");
                return;
            }
            Add($"Player {_winner} wins the game");
            _player1.Log.AddRange(LogList);
            _player2.Log.AddRange(LogList);
        }

        /// <summary>
        /// Wrapper for adding to the Log
        /// </summary>
        /// <param name="message"></param>
        private void Add(string message)
        {
            Console.WriteLine(message);
            LogList.Add(message);
        }

        public void WriteLine(object text)
        {

        }

        public BattleResult GetResult()
        {
            if (_winner == null)
            {
                return new BattleResult(LogList);
            }
            return _winner == _player1.Username ? new BattleResult(_winner, _player2.Username, LogList) : new BattleResult(_winner, _player1.Username, LogList);
        }

        public void AddStartInfo()
        {
            Add("Let the rumbles begin!");
            Add($"Player {_player1.Username} VS Player {_player2.Username}");
        }
    }
}