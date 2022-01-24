using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ServerModule.Mapping;
using ServerModule.Requests;
using ServerModule.Responses;
using ServerModule.Utility;
using Char = ServerModule.Utility.Char;

namespace ServerModule.Tcp.Client
{
    /// <summary>
    ///     Wrapper Class for System.Net.Sockets.TcpClient
    /// </summary>
    public class TcpClient : ITcpClient
    {
        private static readonly IMap Map = DependencyService.GetInstance<IMap>();
        private readonly System.Net.Sockets.TcpClient _client;

        public TcpClient(System.Net.Sockets.TcpClient client)
        {
            _client = client;
        }

        public Request ReadRequest()
        {
            // Read request
            // See: https://developer.mozilla.org/en-US/docs/Web/HTTP/Messages
            /*
             * Request composes of:
             *  1. start-line
             *  2. (opt.) headers
             *  3. blank line (indicates all metadata for the request has been sent)
             *  4. (opt.) body
             */

            // Set timeout
            _client.ReceiveTimeout = 5000;
            // leaveOpen: true, because if I dispose the dataStream now, it will also close the underlying NetworkStream of the client
            using var reader = new StreamReader(_client.GetStream(), leaveOpen: true);


            // start-line variables
            Method method;
            string target;
            string version;

            // headers
            var headers = new Dictionary<string, string>();

            // payload variables must be initialized because it may be empty
            var contentLength = 0;
            string payload = null;
            string requestParam = null;
            string pathVariable = null;

            // Read start-line
            try
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) return null;
                line = line.Trim();

                // start-line consists of three elements: an HTTP-Method, Request-Target and HTTP-Version
                var startLine = line.Split(Utils.GetChar(Char.WhiteSpace));
                method = Utils.GetMethod(startLine[0]);
                target = startLine[1];
                version = startLine[2];

                int indexOfQuestionMark;
                // only Get-Methods have "?" with Request parameters
                if (method == Method.Get &&
                    (indexOfQuestionMark = target.IndexOf(Utils.GetChar(Char.QuestionMark))) != -1)
                {
                    requestParam = target[(indexOfQuestionMark + 1)..];
                    target = target[..indexOfQuestionMark];
                }

                /*
                 * target can have multiple "/", so check if those paths exists 
                 * only supported up to two "/"
                 */
                if (!Map.Contains(method, target))
                {
                    pathVariable = target[(target.LastIndexOf(Utils.GetChar(Char.Slash)) + 1)..];
                    /*
                     * Use Math.Max to counter negative values in paths
                     * example: "transactions" -> pathVariable = "transactions"; target.Substring(0,-1) throws error
                     */
                    target = target[..Math.Max(target.LastIndexOf(Utils.GetChar(Char.Slash)), 0)];
                    // if path still not found, see ServiceHandler.cs
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            // Read HTTP headers until blank line
            try
            {
                string line;
                while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
                {
                    var header = line.Split(Utils.GetChar(Char.Colon));
                    headers.Add(header[0].Trim(), header[1].Trim());
                    if (header[0] == "Content-Length") contentLength = int.Parse(header[1]);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            // Read HTTP body (if exists)
            if (contentLength > 0 && headers.ContainsKey("Content-Type"))
            {
                var body = new StringBuilder();
                const int bufferSize = 1024;
                var buffer = new char[bufferSize];
                var bytesReadTotal = 0;
                while (bytesReadTotal < contentLength)
                    try
                    {
                        var bytesRead = reader.Read(buffer, 0, bufferSize);
                        if (bytesRead == 0) break;
                        bytesReadTotal += bytesRead;
                        body.Append(buffer, 0, bytesRead);
                    }
                    // IOException can occur when there is a mismatch of the 'Content-Length' because a different encoding is used
                    // Sending a 'plain/text' payload with special characters (äüö...) is an example of this
                    catch (IOException)
                    {
                        break;
                    }

                payload = body.ToString();
            }

            var request = new Request(method, target, version, headers, payload, pathVariable, requestParam);
            return request;
        }

        public void SendResponse(in Response response)
        {
            // Send request
            // See: https://developer.mozilla.org/en-US/docs/Web/HTTP/Messages
            // and: https://developer.mozilla.org/en-US/docs/Glossary/Response_header
            // and: https://www.w3.org/Protocols/HTTP/1.1/draft-ietf-http-v11-spec-01

            using var writer = new StreamWriter(_client.GetStream()) { AutoFlush = true };
            writer.WriteLine($"HTTP/1.1 {response.StatusCode} {response.StatusName}");
            writer.WriteLine("Server: MTCG-Docker-Service");
            // See: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Connection
            // and: https://stackoverflow.com/a/38648237
            writer.WriteLine("Connection: close");
            // Format date using RFC1123 pattern
            // See: https://docs.microsoft.com/en-us/dotnet/api/system.datetime.getdatetimeformats?view=net-6.0
            // and: https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings
            writer.WriteLine("Date: " + DateTime.Now.ToString("R") + "");
            // Check if additional header should be sent
            if (response.Headers is { Count: > 0 } headers)
                foreach ((var key, var value) in headers)
                    writer.WriteLine($"{key}: {value}");
            // Check if response has body
            if (!response.ContainsBody)
            {
                writer.WriteLine();
            }
            else
            {
                // Send payload
                // See: https://riptutorial.com/dot-net/example/88/sending-a-post-request-with-a-string-payload-using-system-net-webclient
                // and: https://stackoverflow.com/a/4414118/12347616
                // and: https://blog.j2i.net/2021/10/12/simple-http-server-in-net/
                writer.WriteLine($"Content-Type: {response.ContentType}");
                var payload = Encoding.UTF8.GetBytes(response.Payload);
                writer.WriteLine($"Content-Length: {payload.Length}");
                writer.WriteLine();
                writer.WriteLine(Encoding.UTF8.GetString(payload));
            }
        }

        public void Close()
        {
            _client.Close();
        }

        ~TcpClient()
        {
            Close();
        }
    }
}