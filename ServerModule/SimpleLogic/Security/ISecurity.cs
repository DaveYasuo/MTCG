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
        /// <summary>
        /// Checks credentials and add it to the Session (Hashset)
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns the authorization token by success, else null</returns>
        string Login(User user);

        AuthToken GetTokenDetails(string token);
    }
}