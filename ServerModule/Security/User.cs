namespace ServerModule.Security
{
    /// <summary>
    /// This class is used to make JSON data (register and login) to User class
    /// </summary>
    public class User
    {
        public string Username { get; }
        public string Password { get; }
        public UserStatus StatusCode { get; set; }

        /// <summary>
        /// Used for ISecurity authentication
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="statusCode"></param>
        public User(string username, string password, UserStatus statusCode)
        {
            Username = username;
            Password = password;
            StatusCode = statusCode;
        }
    }
}
