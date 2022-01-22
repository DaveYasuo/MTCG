namespace MTCG.Models
{
    public interface IProfileData
    {
        public string Username { get; }
        public string Name { get; }
        public string Bio { get; }
        public string Image { get; }
    }
}