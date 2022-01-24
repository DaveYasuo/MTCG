namespace MTCG.Models
{
    public class UserData
    {
        public UserData(string name, string bio, string image)
        {
            Name = name;
            Bio = bio;
            Image = image;
        }

        public string Name { get; }
        public string Bio { get; }
        public string Image { get; }
    }
}