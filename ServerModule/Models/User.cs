namespace ServerModule.Models
{
    /// <summary>
    /// This class is used to make JSON data (register and login) to User class
    /// </summary>
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }

        //public User(){}

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
