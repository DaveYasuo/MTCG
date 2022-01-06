using ServerModule.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;

namespace ServerModule.SimpleLogic.Responses
{
    public class Response
    {
        public bool ContainsBody { get; }
        public bool IsText { get; }
        public bool IsJson { get; }

        public int StatusCode { get; }
        public string StatusName { get; }
        public string Payload { get; }
        public string ContentType { get; }

        private Response(int status)
        {
            ContainsBody = false;
            StatusCode = status;
            StatusName = Utils.GetResponseStatusText(status);
        }

        private Response(Status status)
        {
            ContainsBody = false;
            StatusCode = (int)status;
            StatusName = Utils.GetResponseStatusText(StatusCode);
        }

        private Response(Dictionary<string, object> json, Status status = Responses.Status.Ok)
        {
            StatusCode = (int)status;
            StatusName = Utils.GetResponseStatusText(StatusCode);
            ContainsBody = true;
            IsJson = true;
            Payload = JsonSerializer.Serialize(json);
            ContentType = "application/json";
        }

        private Response(string plainText, Status status = Responses.Status.Ok)
        {
            StatusCode = (int)status;
            StatusName = Utils.GetResponseStatusText(StatusCode);
            ContainsBody = true;
            IsText = true;
            Payload = plainText;
            ContentType = "text/plain; charset=utf-8";
        }

        /// <summary>
        /// Returns a new response instance with no content.
        /// </summary>
        /// <param name="status"></param>
        /// <returns>Returns the given response with no content.</returns>
        public static Response Status(Status status)
        {
            return new Response(status);
        }

        /// <summary>
        /// Returns a new response instance with JSON as payload and default status code 200 which can be overwritten.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="status"></param>
        /// <returns>Returns a new response instance with JSON as payload and the given status code.</returns>
        public static Response Json(Dictionary<string, object> json, Status status = Responses.Status.Ok)
        {
            return new Response(json, status);
        }

        /// <summary>
        /// Returns a new response instance with utf-8 plaintext as payload and default status code 200 which can be overwritten.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="status"></param>
        /// <returns>Returns a new response instance with utf-8 plaintext as payload and the given status code.</returns>
        public static Response PlainText(string plainText, Status status = Responses.Status.Ok)
        {
            return new Response(plainText, status);
        }
    }
}