using System;
using System.Text.Json;

namespace Data.Users
{
    public enum Role
    {
        User,
        Admin
    }

    public enum TokenType
    {
        Basic
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public (bool, string) Register()
        {
            return Authentication
        }

        public static void PrintUserData(string data)
        {
            User newUser = JsonSerializer.Deserialize<User>(data);
            try
            {
                //DB.InsertUser(newUser);
                Console.WriteLine($"User:{newUser?.Username}");
                Console.WriteLine($"Password:{newUser?.Password}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
