namespace ServerModule.Database.Schemas
{
    public class Credentials
    {
        public Credentials(string username, string password, string role)
        {
            Username = username;
            Password = password;
            Role = role;
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}