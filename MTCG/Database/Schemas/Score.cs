namespace MTCG.Database.Schemas
{
    public class Score : IStats
    {
        public long Rank { get; }
        public string Username { get; }
        public short Elo { get; }
        public int Wins { get; }
        public int Losses { get; }
        public int Draws { get; }

        /// <summary>
        /// Use this ctor for getting stats for the scoreboard
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="username"></param>
        /// <param name="elo"></param>
        /// <param name="wins"></param>
        /// <param name="losses"></param>
        /// <param name="draws"></param>
        public Score( long rank, string username, short elo, int wins, int losses, int draws)
        {
            Rank = rank;
            Username = username;
            Elo = elo;
            Wins = wins;
            Losses = losses;
            Draws = draws;
        }
    }
}