using DebugAndTrace;
using MTCG.Data.Cards;
using MTCG.Data.Users;
using MTCG.Data.Utils;
using MTCG.Logging;

namespace MTCG.BattleLogic
{
    class Battle
    {
        private readonly IPlayer _player1;
        private readonly IPlayer _player2;
        private readonly IBattleLog _battleLog;

        public Battle(IPlayer player1, IPlayer player2, IBattleLog battleLog)
        {
            _player1 = player1;
            _player2 = player2;
            _battleLog = battleLog;
        }

        public void StartGame()
        {
            _battleLog.AddStartInfo();
            for (int round = 1; round <= 100; round++)
            {
                ICard card1 = _player1.GetRandomCard();
                ICard card2 = _player2.GetRandomCard();
                (float damage1, float damage2) = Utility.CalculateDamage(card1, card2);
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
            return  _battleLog.GetResult();;
        }
    }
}
