using System.Collections.Generic;
using ServerModule.Security;

namespace MTCG.Database.Schemas
{
    public class Credentials
    {
        // Conversion performance
        // See: https://stackoverflow.com/a/38711
        private static readonly Dictionary<string, Role> Roles = new()
        {
            { "User", Role.User },
            { "Admin", Role.Admin }
        };


        public Credentials(string token, string username, string password, string role)
        {
            Token = token;
            Username = username;
            Password = password;
            Roles.TryGetValue(role, out var value);
            Role = value;
        }

        public Credentials(string token, string username, string password, Role role)
        {
            Token = token;
            Username = username;
            Password = password;
            Role = role;
        }

        public string Token { get; }
        public string Username { get; }
        public string Password { get; }
        public Role Role { get; }
    }
}