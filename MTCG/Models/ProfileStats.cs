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
        // Win Loss Ratio 
        // See: https://en.wikipedia.org/wiki/Winning_percentage
        public float WinLossRatio
        {
            get
            {
                if (Wins == 0 && Draws == 0) return 0;
                // Float suffix
                // See: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types
                return ((Wins + 0.5F * Draws) / GamesPlayed);
            }
        }

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