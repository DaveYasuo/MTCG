using System.Collections.Generic;
using MTCG.BattleLogic;
using MTCG.Data.Users;

namespace MTCG.Logging
{
    public class BattleLog : IBattleLog
    {
        private readonly IPlayer _player1;
        private readonly IPlayer _player2;
        private CardLog _log;
        private string _winner;

        public BattleLog(IPlayer player1, IPlayer player2)
        {
            _player1 = player1;
            _player2 = player2;
            LogList = new List<object>();
        }

        public List<object> LogList { get; }

        /// <summary>
        ///     Adds an intro to the LogList.
        /// </summary>
        public void AddStartInfo()
        {
            Add($"Player {_player1.Username} VS Player {_player2.Username}");
        }

        /// <summary>
        ///     Creates a new CardLog and log base damages to it.
        /// </summary>
        /// <param name="myDamage"></param>
        /// <param name="otherDamage"></param>
        public void AddBaseDamage(float myDamage, float otherDamage)
        {
            _log = new CardLog
            {
                Base = myDamage + " VS " + otherDamage
            };
        }

        /// <summary>
        ///     Adds elemental advantages to the existing CardLog.
        /// </summary>
        /// <param name="myElement"></param>
        /// <param name="otherElement"></param>
        public void AddElementReaction(string myElement, string otherElement)
        {
            _log.Description.Add(myElement + " is effective against " + otherElement + ", so damage is doubled");
            _log.Description.Add(otherElement + " is not effective against " + myElement + ", so damage is halved");
        }

        /// <summary>
        ///     Add the specific specialty information as a string to the CardLog in the description section.
        /// </summary>
        /// <param name="infoMessage"></param>
        public void AddEffectInfo(string infoMessage)
        {
            _log.Description.Add(infoMessage);
        }

        /// <summary>
        ///     Adds the resulting damages to the CardLog.
        /// </summary>
        /// <param name="myDamage"></param>
        /// <param name="otherDamage"></param>
        public void AddEffectiveDamage(float myDamage, float otherDamage)
        {
            _log.Effective = myDamage + " VS " + otherDamage;
        }

        public void AddBonusDamage(int bonusDmg1, int bonusDmg2)
        {
            _log.Description.Add($"{_player1.LastPlayedCard} gets additional {bonusDmg1} damage boost.");
            _log.Description.Add($"{_player2.LastPlayedCard} gets additional {bonusDmg2} damage boost.");
        }

        /// <summary>
        ///     Creates a new RoundLog object and adds it to the LogList.
        /// </summary>
        /// <param name="damage1"></param>
        /// <param name="damage2"></param>
        /// <param name="round"></param>
        public void AddRound(float damage1, float damage2, int round)
        {
            _winner = damage1.CompareTo(damage2) switch
            {
                > 0 => _player1.Username,
                < 0 => _player2.Username,
                _ => null
            };
            var roundLog = new RoundLog
            {
                Round = round,
                Title = $"{_player1.LastPlayedCard} VS {_player2.LastPlayedCard}",
                Body = _log,
                RemainingCards = $" {_player1.Cards.Count} VS {_player2.Cards.Count}",
                Result = _winner is { } ? $"{_winner} wins the round" : "Draw"
            };
            Add(roundLog);
            if (_player1.Cards.Count == 0 || _player2.Cards.Count == 0) AddResult();
            if (round != 100) return;
            _winner = null;
            AddResult();
        }

        /// <summary>
        ///     Gets the current BattleLog and the results.
        /// </summary>
        /// <returns>A BattleResult object containing the battle results.</returns>
        public BattleResult GetResult()
        {
            return _winner == null
                ? new BattleResult()
                : new BattleResult(_winner, _winner == _player1.Username ? _player2.Username : _player1.Username);
        }

        /// <summary>
        ///     Wrapper for adding to the Log
        /// </summary>
        /// <param name="message"></param>
        private void Add(object message)
        {
            LogList.Add(message);
        }

        /// <summary>
        ///     Adds the battle result to the LogList and then to the player's log.
        /// </summary>
        private void AddResult()
        {
            Add(_winner == null ? "Draw Game" : $"Player {_winner} wins the game");
            _player1.Log.AddRange(LogList);
            _player2.Log.AddRange(LogList);
        }
    }
}