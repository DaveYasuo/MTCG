namespace MTCG.BattleLogic
{
    public class BattleResult
    {
        public bool Draw { get; }
        public string Winner { get; }
        public string Loser { get; }


        public BattleResult()
        {
            Draw = true;
        }

        public BattleResult(string winner, string loser)
        {
            Draw = false;
            Winner = winner;
            Loser = loser;
        }
    }
}