namespace MTCG.Models
{
    public interface IStats
    {
        public string Username { get; }
        public short Elo { get; }
        public int Wins { get; }
        public int Losses { get; }
        public int Draws { get; }
        public long GamesPlayed => Wins + Losses + Draws;
    }
}