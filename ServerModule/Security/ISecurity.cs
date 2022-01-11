using System.Collections.Generic;
using ServerModule.Models;
using ServerModule.Utility;

namespace ServerModule.Security
{
    public interface ISecurity
    {
        public Dictionary<Method, List<string>> SecuredPaths();
        bool Authenticate(string type, string token);
        bool Register(User user);
        bool CheckCredentials(string userUsername, string userPassword);
        string Login(User user);
        AuthToken GetTokenDetails(string token);
        bool UpdateGameStatus(string token, bool setStatus);
    }
}