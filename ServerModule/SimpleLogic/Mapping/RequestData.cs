using System.Collections.Generic;

namespace ServerModule.SimpleLogic.Mapping
{
    public class RequestData
    {
        public string Username { get; }
        public string Payload { get; }
        public string PathVariable { get; }
        public Dictionary<string, string> RequestParam { get; }

        public RequestData(string username, string payload, string pathVariable, string requestParam)
        {

            Dictionary<string, string> temp = new Dictionary<string, string>();
            if (requestParam != null)
            {
                string[] entries = requestParam.Split('&');
                foreach (string entry in entries)
                {
                    string[] tmp = entry.Split('=');
                    if (tmp.Length != 2) continue;
                    string key = tmp[0];
                    string val = tmp[1];
                    temp.Add(key, val);
                }
            }
            RequestParam = temp;
            PathVariable = pathVariable;
            Payload = payload;
            Username = username;
        }

    }
}