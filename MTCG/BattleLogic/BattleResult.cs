using System.Collections.Generic;

namespace MTCG.BattleLogic
{
    public class BattleResult
    {
        public bool Draw { get; }
        public string Winner { get; }
        public string Loser { get; }
        public List<object> Log { get; }


        public BattleResult(List<object> log)
        {
            Log = log;
            Draw = true;
        }

        public BattleResult(string winner, string loser, List<object> log)
        {
            Draw = false;
            Winner = winner;
            Loser = loser;
            Log = log;
        }
    }
}