using System;
using System.Collections.Generic;
using System.Text.Json;
using ServerModule.Database.Models;
using ServerModule.Utility;
using Char = ServerModule.Utility.Char;

namespace ServerModule.SimpleLogic.Requests
{
    public interface IRequest
    {
        public Method Method { get; }
        /// <summary>
        /// Target is the path
        /// </summary>
        public string Target { get; }
        public string Version { get; }
        public Dictionary<string, string> Headers { get; }
        public string Payload { get; }
        public string PathVariable { get; }
        public string RequestParam { get; }
        /// <summary>
        /// Extendable function for checking Payload for different Content-Types
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="contentType"></param>
        public static void CheckPayload(ref string payload, string contentType)
        {
            switch (contentType)
            {
                case "application/json":
                    // Support for single values: "
                    // See: https://stackoverflow.com/questions/13318420/is-a-single-string-value-considered-valid-json?lq=1#:~:text=As%20for%20new%20JSON%20RFC,an%20object%20or%20an%20array.
                    if (payload.StartsWith(Utils.GetChar(Char.DoubleQuote)))
                    {
                        payload = "{\"value\":" + payload + "}";
                    }
                    //payload = JsonSerializer.Deserialize<Dictionary<string, object>>((string)payload);
                    break;
                // Other Content-Types are not supported
                default:
                    payload = null;
                    break;
            }
        }
    }
}