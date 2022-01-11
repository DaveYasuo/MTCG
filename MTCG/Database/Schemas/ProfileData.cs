namespace MTCG.Database.Schemas
{
    public class ProfileData : IProfileData
    {
        public string Username { get; }
        public string Name { get; }
        public string Bio { get; }
        public string Image { get; }

        /// <summary>
        /// Use this ctor for getting profile data from database.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="name"></param>
        /// <param name="bio"></param>
        /// <param name="image"></param>
        public ProfileData(string username, string name, string bio, string image)
        {
            Username = username;
            Name = name;
            Bio = bio;
            Image = image;
        }

    }
}