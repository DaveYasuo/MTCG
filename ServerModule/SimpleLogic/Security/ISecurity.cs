using System.Collections.Generic;
using ServerModule.Database.Models;
using ServerModule.Utility;

namespace ServerModule.SimpleLogic.Security
{
    public interface ISecurity
    {
        public Dictionary<Method, List<string>> SecuredPaths();
        bool Authenticate(string type, string token);
        (bool, string) Register(User user);
    }
}