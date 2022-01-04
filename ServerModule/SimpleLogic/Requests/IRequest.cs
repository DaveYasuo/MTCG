using System;
using System.Collections.Generic;
using System.Text.Json;
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
        public object Payload { get; }
        public string PathVariable { get; }
        public string RequestParam { get; }
        public static void ParseBody(ref object payload, string contentType)
        {
            switch (contentType)
            {
                case "text/plain":
                    payload ??= "";
                    break;
                case "application/json":
                    payload ??= "{}";
                    try
                    {
                        // Support for single values: "
                        // See: https://stackoverflow.com/questions/13318420/is-a-single-string-value-considered-valid-json?lq=1#:~:text=As%20for%20new%20JSON%20RFC,an%20object%20or%20an%20array.
                        if (((string)payload).StartsWith(Utils.GetChar(Char.DoubleQuote)))
                        {
                            payload = "{\"value\":" + payload + "}";
                        }

                        // Support for arrays: [
                        if (((string)payload).StartsWith(Utils.GetChar(Char.OpenBracket)))
                        {
                            payload = "{\"array\":" + payload + "}";
                        }

                        payload = JsonSerializer.Deserialize<Dictionary<string, object>>((string)payload);
                    }
                    catch (Exception)
                    {
                        // if Deserialization failed
                        payload = null;
                    }
                    break;
                default:
                    payload = null;
                    break;
            }
        }
    }
}