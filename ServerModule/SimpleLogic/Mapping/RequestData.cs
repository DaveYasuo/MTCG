using System.Collections.Generic;
using ServerModule.SimpleLogic.Security;

namespace ServerModule.SimpleLogic.Mapping
{
    public class RequestData
    {
        public AuthToken Authentication { get; }
        public string Payload { get; }
        public string PathVariable { get; }
        public Dictionary<string, string> RequestParam { get; }

        public RequestData(AuthToken authentication, string payload, string pathVariable, string requestParam)
        {


            RequestParam = SetRequestParameter(requestParam);
            PathVariable = pathVariable;
            Payload = payload;
            Authentication = authentication;
        }

        private static Dictionary<string, string> SetRequestParameter(string requestParam)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            if (requestParam != null)
            {
                string[] entries = requestParam.Split('&');
                foreach (string entry in entries)
                {
                    string[] tmp = entry.Split('=');
                    if (tmp.Length != 2) continue;
                    string key = tmp[0];
                    string val = tmp[1];
                    parameter.Add(key, val);
                }
            }
            return parameter;
        }
    }
}