namespace ServerModule.Database.Schemas
{
    public class UserSchema
    {
        public UserSchema(string username, string password, string roleType)
        {
            Username = username;
            Password = password;
            RoleType = roleType;
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string RoleType { get; set; }
    }
}