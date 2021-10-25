using System;
using System.Text.Json;

namespace Data.Users
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }

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
