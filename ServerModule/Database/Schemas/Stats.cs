namespace ServerModule.Database.Schemas
{
    public class Stats : IStats
    {
        public string Username { get; }
        public int Elo { get; }
        public int Wins { get; }
        public int Losses { get; }
        public int Draws { get; }

        /// <summary>
        /// Use this ctor for getting stats for the scoreboard
        /// </summary>
        /// <param name="username"></param>
        /// <param name="elo"></param>
        /// <param name="wins"></param>
        /// <param name="losses"></param>
        /// <param name="draws"></param>
        public Stats(string username, int elo, int wins, int losses, int draws)
        {
            Username = username;
            Elo = elo;
            Wins = wins;
            Losses = losses;
            Draws = draws;
        }
    }
}