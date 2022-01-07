﻿using System;
using System.Text.Json;
using ServerModule.SimpleLogic.Security;

namespace ServerModule.Database.Models
{
    /// <summary>
    /// This class is used to make JSON data (register and login) to User class
    /// </summary>
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public User(){}

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Register()
        {
            return Authentication.Register(this);
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

        public string Login()
        {
            return Authentication.Login(this);
        }
    }
}
