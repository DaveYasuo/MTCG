using ServerModule.SimpleLogic.Mapping;
using ServerModule.SimpleLogic.Requests;
using ServerModule.SimpleLogic.Responses;
using ServerModule.SimpleLogic.Security;
using ServerModule.Utility;

namespace ServerModule.SimpleLogic.Handler
{
    public class ServiceHandler
    {
        private readonly IMapping _mapping;

        /**
         * Unklar: Authorization
         * Nimmt die Parameter vom Client (durch die Implementation von ServerModule.Tcp) entgegen
         * Erstellt eine Instanz von RequestHandler, übergibt die Daten, Rückgabewert wird als Response
         * wieder an den Client gesendet.
         **/

        public ServiceHandler(IMapping mapping)
        {
            _mapping = mapping;
        }

        public Response HandleRequest(Request request)
        {
            // See: https://developer.mozilla.org/de/docs/Web/HTTP/Status

            // if Request has unsupported syntax
            if (request == null) return Response.Status(Status.BadRequest);
            // if path is unsupported
            if (!_mapping.Contains(request.Method, request.Target)) return Response.Status(Status.NotFound);
            // if HTTP Version is unsupported
            if (request.Version != "HTTP/1.1") return Response.Status(Status.HttpVersionNotSupported);
            // if Method is not supported
            // The server must generate an Allow header field in a 405 status code response.
            // The field must contain a list of methods that the target resource currently supports. todo
            if (request.Method is Method.Patch or Method.Error) return Response.Status(Status.MethodNotAllowed);

            // Proceed handling Request:
            // Check if path is secured, if not just invoke the corresponding function
            if (!Authentication.PathIsSecured(request.Method, request.Target)) return _mapping.InvokeMethod(request.Method, request.Target, null, request.Payload, request.PathVariable, request.RequestParam);
            // Check if Authorization header was send
            if (!request.Headers.ContainsKey("Authorization")) return Response.Status(Status.Unauthorized);
            // If so, check credentials
            string[] line = request.Headers["Authorization"].Split(Utils.GetChar(Char.WhiteSpace));
            if (line.Length != 2) return Response.Status(Status.Forbidden);
            string type = line[0];
            string token = line[1];
            if (Authentication.Check(type, token))
            {
                // invoke the corresponding function
                return _mapping.InvokeMethod(request.Method, request.Target, token[..^10], request.Payload, request.PathVariable, request.RequestParam);
            }
            return Response.Status(Status.Forbidden);
        }
    }
}
