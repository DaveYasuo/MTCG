using System.Collections.Generic;
using ServerModule.Security;

namespace ServerModule.Mapping
{
    public class RequestData
    {
        public RequestData(AuthToken authentication, string payload, string pathVariable, string requestParam)
        {
            RequestParam = SetRequestParameter(requestParam);
            PathVariable = pathVariable;
            Payload = payload;
            Authentication = authentication;
        }

        public AuthToken Authentication { get; }
        public string Payload { get; }
        public string PathVariable { get; }
        public Dictionary<string, string> RequestParam { get; }

        private static Dictionary<string, string> SetRequestParameter(string requestParam)
        {
            var parameter = new Dictionary<string, string>();
            if (requestParam == null) return parameter;
            var entries = requestParam.Split('&');
            foreach (var entry in entries)
            {
                var tmp = entry.Split('=');
                if (tmp.Length != 2) continue;
                var key = tmp[0];
                var val = tmp[1];
                parameter.Add(key, val);
            }

            return parameter;
        }
    }
}