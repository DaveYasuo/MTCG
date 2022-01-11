using System.Collections.Generic;
using ServerModule.Utility;

namespace ServerModule.Requests
{
    public class Request : IRequest
    {
        public Method Method { get; }
        public string Target { get; }
        public string Version { get; }
        public Dictionary<string, string> Headers { get; }
        public string Payload { get; }
        public string PathVariable { get; }
        public string RequestParam { get; }

        public Request(Method method, string target, string version, Dictionary<string, string> headers, string payload, string pathVariable, string requestParam)
        {
            Method = method;
            Target = target;
            Version = version;
            Headers = headers;
            Payload = payload;
            PathVariable = pathVariable;
            RequestParam = requestParam;
        }
    }
}