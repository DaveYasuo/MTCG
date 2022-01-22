namespace MTCG.Models
{
    public class ProfileStats : IStats
    {
        public string Username { get; }
        public short Elo { get; }
        public int Wins { get; }
        public int Losses { get; }
        public int Draws { get; }
        public long Coins { get; }
        public long GamesPlayed => Wins + Losses + Draws;

        /// <summary>
        /// Use this ctor for getting profile stats from database.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="elo"></param>
        /// <param name="wins"></param>
        /// <param name="losses"></param>
        /// <param name="draws"></param>
        /// <param name="coins"></param>
        public ProfileStats(string username, short elo, int wins, int losses, int draws, long coins)
        {
            Username = username;
            Elo = elo;
            Wins = wins;
            Losses = losses;
            Draws = draws;
            Coins = coins;
        }
    }
}