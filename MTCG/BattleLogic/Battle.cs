﻿using MTCG.Data.Cards;
using MTCG.Data.Users;
using MTCG.Logging;
using MTCG.Utilities;

namespace MTCG.BattleLogic
{
    public class Battle
    {
        private readonly IPlayer _player1;
        private readonly IPlayer _player2;
        private readonly IBattleLog _battleLog;

        public Battle(IPlayer player1, IPlayer player2)
        {
            _player1 = player1;
            _player2 = player2;
            _battleLog = new BattleLog(player1, player2);
        }

        public void StartGame()
        {
            _battleLog.AddStartInfo();
            for (int round = 1; round <= 100; round++)
            {
                ICard card1 = _player1.GetRandomCard();
                ICard card2 = _player2.GetRandomCard();
                (float damage1, float damage2) = Utility.CalculateDamage(card1, card2, in _battleLog);
                if (round % 10 == 0) Utility.BonusRound(ref damage1, ref damage2, in _battleLog);
                _battleLog.AddEffectiveDamage(damage1, damage2);
                switch (damage1.CompareTo(damage2))
                {
                    case > 0:
                        {
                            _player1.Add(card2);
                            _player2.Remove(card2);
                            break;
                        }
                    case < 0:
                        {
                            _player2.Add(card1);
                            _player1.Remove(card1);
                            break;
                        }
                }
                _battleLog.AddRound(damage1, damage2, round);
                if (_player1.Cards.Count == 0 || _player2.Cards.Count == 0)
                {
                    break;
                }
            }
        }
        public BattleResult GetResult()
        {
            return _battleLog.GetResult();
        }
    }
}
