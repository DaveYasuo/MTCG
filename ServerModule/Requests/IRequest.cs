using System.Collections.Generic;
using ServerModule.Utility;

namespace ServerModule.Requests
{
    public interface IRequest
    {
        public Method Method { get; }

        /// <summary>
        ///     Target is the path
        /// </summary>
        public string Target { get; }

        public string Version { get; }
        public Dictionary<string, string> Headers { get; }
        public string Payload { get; }
        public string PathVariable { get; }
        public string RequestParam { get; }
    }
}