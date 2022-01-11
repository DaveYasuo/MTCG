namespace ServerModule.Security
{
    public class AuthToken
    {
        public AuthToken(string username, string token)
        {
            Username = username;
            Token = token;
        }

        public string Username { get; }
        public string Token { get; }
    }
}