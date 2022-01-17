using System.Collections.Generic;

namespace MTCG.BattleLogic
{
    public class BattleResult
    {
        public bool Draw { get; }
        public string Winner { get; }
        public string Loser { get; }
        public List<string> Log { get; }


        public BattleResult(List<string> log)
        {
            Log = log;
            Draw = true;
        }

        public BattleResult(string winner, string loser, List<string> log)
        {
            Draw = false;
            Winner = winner;
            Loser = loser;
            Log = log;
        }
    }
}