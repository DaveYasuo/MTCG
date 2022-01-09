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

        /// <summary>
        /// Checks if User is logged in and is also (still) in the database
        /// </summary>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <returns>True if is logged in, else false</returns>
        public bool Authenticate(string type, string token)
        {
            // Authorization Header
            // See: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Authorization
            bool ewr = type == "Basic";
            bool awer = token.EndsWith("-mtcgToken");
            bool rwer = _basicTokens.Contains(token);
            bool wer =DataHandler.ContainsToken(token);
            return type == "Basic" && token.EndsWith("-mtcgToken") && _basicTokens.Contains(token) && DataHandler.ContainsToken(token);
        }

        /// <summary>
        /// Register User to the database.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns true if register is success else false.</returns>
        public bool Register(User user)
        {
            // check if User already exists
            if (DataHandler.GetUser(user.Username) != null) return false;
            string pwHash = GenerateHash(user.Password);
            // i think authToken should be generated when trying to log in
            //string token = GenerateToken(user.Username);
            Credentials newUser = user.Username == "admin"
                ? new Credentials(null, user.Username, pwHash, Role.Admin)
                : new Credentials(null, user.Username, pwHash, Role.User);
            return DataHandler.AddUser(newUser);
        }

        public string Login(User user)
        {
            if (!CheckCredentials(user.Username, user.Password)) return null;
            string token = GenerateToken(user.Username);
            return AddToken(token, user.Username) ? token : null;
        }

        public AuthToken GetTokenDetails(string token)
        {
            int indexOfToken = token.LastIndexOf("-mtcgToken", StringComparison.Ordinal);
            string username = token[..indexOfToken];
            return new AuthToken(username, token);
        }

        private bool AddToken(string token, string username)
        {
            if (!DataHandler.AddTokenToUser(token, username)) return false;
            _basicTokens.Add(token);
            return true;
        }

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