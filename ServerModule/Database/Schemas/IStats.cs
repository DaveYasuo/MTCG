namespace ServerModule.Database.Schemas
{
    public interface IStats
    {
        public string Username { get; }
        public int Elo { get; }
        public int Wins { get; }
        public int Losses { get; }
        public int Draws { get; }
    }
}