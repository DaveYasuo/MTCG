namespace ServerModule.Database.Schemas
{
    public class Profile
    {
        public string Username { get; }
        public string Name { get; }
        public string Bio { get; }
        public string Image { get; }
        public int Elo { get; }
        public int Wins { get; }
        public int Losses { get; }
        public int Draws { get; }
        public long Coins { get; }
        //public long GamesPlayed => Wins + Losses + Draws;

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

        //public Profile(string username, int elo, int wins, int losses, int draws)
        //{
        //    Username = username;
        //    Elo = elo;
        //    Wins = wins;
        //    Losses = losses;
        //    Draws = draws;
        //    Name = string.Empty;
        //    Bio = string.Empty;
        //    Image = string.Empty;
        //}

        //public Profile(
        //    string username, int elo, int wins, int losses, int draws, long coins,
        //    string name, string bio, string image
        //)
        //{
        //    Username = username;
        //    Name = name;
        //    Bio = bio;
        //    Image = image;
        //    Elo = elo;
        //    Wins = wins;
        //    Losses = losses;
        //    Draws = draws;
        //    Coins = coins;
        //}
    }
}