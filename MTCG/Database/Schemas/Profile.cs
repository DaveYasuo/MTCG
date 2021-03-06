using MTCG.Models;

namespace MTCG.Database.Schemas
{
    public class Profile : IProfileData, IStats
    {
        /// <summary>
        ///     Use this ctor for generating new profile for database insertion
        /// </summary>
        /// <param name="username"></param>
        public Profile(string username)
        {
            Username = username;
            Elo = 100;
            Wins = 0;
            Losses = 0;
            Draws = 0;
            Coins = 20;
            Name = "No name given";
            Bio = "No Bio";
            Image = "No Image";
        }

        /// <summary>
        ///     Use this ctor for getting all data from profile table
        /// </summary>
        /// <param name="username"></param>
        /// <param name="name"></param>
        /// <param name="bio"></param>
        /// <param name="image"></param>
        /// <param name="elo"></param>
        /// <param name="wins"></param>
        /// <param name="losses"></param>
        /// <param name="draws"></param>
        /// <param name="coins"></param>
        public Profile(string username, string name, string bio, string image, short elo, int wins, int losses,
            int draws, long coins)
        {
            Username = username;
            Name = name;
            Bio = bio;
            Image = image;
            Elo = elo;
            Wins = wins;
            Losses = losses;
            Draws = draws;
            Coins = coins;
        }

        public long Coins { get; }
        public string Username { get; }
        public string Name { get; }
        public string Bio { get; }
        public string Image { get; }
        public short Elo { get; }
        public int Wins { get; }
        public int Losses { get; }
        public int Draws { get; }
    }
}