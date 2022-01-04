using System.Collections.Generic;
using ServerModule.Utility;

namespace ServerModule.SimpleLogic.Security
{
    public interface ISecurity
    {
        public Dictionary<Method, List<string>> SecuredPaths();
        bool Authenticate(string type, string token);
    }
}