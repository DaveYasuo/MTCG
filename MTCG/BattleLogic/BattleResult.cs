namespace MTCG.BattleLogic
{
    public class BattleResult
    {
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

        public bool Draw { get; }
        public string Winner { get; }
        public string Loser { get; }
    }
}