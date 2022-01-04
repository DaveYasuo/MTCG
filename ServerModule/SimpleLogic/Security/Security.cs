using System.Collections.Generic;
using ServerModule.Utility;

namespace ServerModule.SimpleLogic.Security
{
    public class Security : ISecurity
    {
        // Using Hashset is more efficient with constant time
        // See: https://stackoverflow.com/a/17278638
        private HashSet<string> _basicTokens = new();

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

        public bool Authenticate(string type, string token)
        {
            // to add another type, a new Hashset is needed
            return type == "Basic" && _basicTokens.Contains(token);
        }

    }
}