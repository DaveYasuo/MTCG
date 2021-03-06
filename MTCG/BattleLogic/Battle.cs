using MTCG.Data.Users;
using MTCG.Logging;
using MTCG.Utilities;

namespace MTCG.BattleLogic
{
    public class Battle
    {
        private readonly IBattleLog _battleLog;
        private readonly IPlayer _player1;
        private readonly IPlayer _player2;

        public Battle(IPlayer player1, IPlayer player2)
        {
            _player1 = player1;
            _player2 = player2;
            _battleLog = new BattleLog(player1, player2);
        }

        public void StartGame()
        {
            _battleLog.AddStartInfo();
            for (var round = 1; round <= 100; round++)
            {
                var card1 = _player1.GetRandomCard();
                var card2 = _player2.GetRandomCard();
                var (damage1, damage2) = DamageUtility.CalculateDamage(card1, card2, in _battleLog);
                if (round % 10 == 0) DamageUtility.BonusRound(ref damage1, ref damage2, in _battleLog);
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
                if (_player1.Cards.Count == 0 || _player2.Cards.Count == 0) break;
            }
        }

        public BattleResult GetResult()
        {
            return _battleLog.GetResult();
        }
    }
}