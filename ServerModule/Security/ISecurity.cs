using System.Collections.Generic;
using ServerModule.Utility;

namespace ServerModule.Security
{
    public interface ISecurity
    {
        public Dictionary<Method, List<string>> SecuredPaths();
        bool Authenticate(string type, string token, UserStatus statusCode);
        bool Register(User user);
        bool CheckCredentials(string userUsername, string userPassword);
        string Login(User user);
        AuthToken GetTokenDetails(string token);
        bool UpdateStatus(string token, UserStatus newStatus, UserStatus oldStatus);
    }
}