using ServerModule.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;

namespace ServerModule.SimpleLogic.Responses
{
    public class Response
    {
        public bool ContainsBody { get; }

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

        private Response(string payload, string contentType, Status status = Responses.Status.Ok)
        {
            StatusCode = (int)status;
            StatusName = Utils.GetResponseStatusText(StatusCode);
            ContainsBody = true;
            Payload = payload;
            ContentType = contentType;
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
        /// Returns a new response instance that converts payload object to JSON string and default status code 200 which can be overwritten.
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="status"></param>
        /// <returns>Returns a new response instance with JSON as payload and the given status code on success, else if an exception is thrown, internal server error.</returns>
        public static Response Json(object payload, Status status = Responses.Status.Ok)
        {
            string json;
            try
            {
                json = JsonSerializer.Serialize(payload);
            }
            catch (Exception)
            {
                return new Response(Responses.Status.InternalServerError);
            }
            return new Response(json, "application/json", status);
        }

        /// <summary>
        /// Returns a new response instance with utf-8 plaintext as payload and default status code 200 which can be overwritten.
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="status"></param>
        /// <returns>Returns a new response instance with utf-8 plaintext as payload and the given status code.</returns>
        public static Response PlainText(string plainText, Status status = Responses.Status.Ok)
        {
            return new Response(plainText, "text/plain; charset=utf-8", status);
        }
    }
}