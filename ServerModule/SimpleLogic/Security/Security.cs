using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using ServerModule.Database.Models;
using ServerModule.Database.Schemas;
using ServerModule.SimpleLogic.Handler;
using ServerModule.Utility;

namespace ServerModule.SimpleLogic.Security
{
    public class Security : ISecurity
    {
        // Using Hashset is more efficient with constant time
        // See: https://stackoverflow.com/a/17278638
        private readonly HashSet<string> _basicTokens = new();

        private readonly Dictionary<Method, List<string>> _protectedPaths = new()
        {
            { Method.Get, new List<string>() { "/cards", "/deck", "/users", "/stats", "/score", "/tradings" } },
            { Method.Post, new List<string>() { "/packages", "/transactions/packages", "/battles", "/tradings" } },
            { Method.Put, new List<string>() { "/deck", "/users" } },
            { Method.Delete, new List<string>() { "/tradings" } },
        };

        public Dictionary<Method, List<string>> SecuredPaths()
        {
            return _protectedPaths;
        }

        // Authorization Header
        // See: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Authorization
        public bool Authenticate(string type, string token)
        {
            return type == "Basic" && token.EndsWith("-mtcgToken") && _basicTokens.Contains(token);
        }

        public string Register(User user)
        {
            // check if User already exists
            if (DataHandler.GetUser(user.Username) != null) return null;
            string pwHash = GenerateHash(user.Password);
            Credentials newUser = user.Username.ToLower() == "admin" ? new Credentials(user.Username, pwHash, "Admin") : new Credentials(user.Username, pwHash, "User");
            // check if User is added successfully to the database
            if (!DataHandler.AddUser(newUser)) return null;
            string token = GenerateToken(user.Username);
            AddToken(token);
            return token;
        }

        public string Login(User user)
        {
            if (!CheckCredentials(user.Username, user.Password)) return null;
            string token = GenerateToken(user.Username);
            AddToken(token);
            return token;
        }

        private void AddToken(string token) => _basicTokens.Add(token);

        /// <summary>
        /// Generates a "username"-mtcgToken
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Returns the generated token</returns>
        private static string GenerateToken(string username) => username + "-mtcgToken";

        private static string GenerateHash(string password)
        {
            // Compute Hash
            // See: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm.computehash?view=net-6.0
            // not used but cool: https://stackoverflow.com/a/8935635 (detect which encoding type was used)
            using SHA256 hashAlgorithm = SHA256.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Create a new StringBuilder to collect the bytes and create a string.
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data and format each one as a hexadecimal string.
            foreach (var b in data) sBuilder.Append(b.ToString("x2"));
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        /// <summary>
        /// Check given username and password with those in the database
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>True if valid, else false.</returns>
        public bool CheckCredentials(string username, string password)
        {
            Credentials check = DataHandler.GetUser(username);
            if (check == null) return false;
            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            string pwHash = GenerateHash(password);
            return comparer.Compare(pwHash, check.Password) == 0;
        }
    }
}