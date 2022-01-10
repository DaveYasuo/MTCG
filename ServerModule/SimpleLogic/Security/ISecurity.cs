using System.Collections.Generic;
using ServerModule.Database.Models;
using ServerModule.Utility;

namespace ServerModule.SimpleLogic.Security
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