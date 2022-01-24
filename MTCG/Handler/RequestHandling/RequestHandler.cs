using System;
using System.Collections.Generic;
using DebugAndTrace;
using MTCG.BattleLogic;
using ServerModule.Mapping;
using ServerModule.Requests;
using ServerModule.Responses;
using ServerModule.Security;
using ServerModule.Utility;

namespace MTCG.Handler.RequestHandling
{
    // Partial classes are so useful
    // See: https://www.geeksforgeeks.org/partial-classes-in-c-sharp/
    // Naming convention
    // See: https://stackoverflow.com/questions/1478610/naming-conventions-for-partial-class-files
    /// <inheritdoc />
    public partial class RequestHandler : IRequestHandler
    {
        private readonly Authentication _auth;

        // Battle Logic
        private readonly GameServer _game;
        private readonly ILogger _log;

        public RequestHandler(GameServer game, Authentication auth, ILogger logger)
        {
            _game = game;
            _auth = auth;
            _log = logger;
        }

        // Function in a dictionary
        // See: https://stackoverflow.com/a/30397975
        // and: https://stackoverflow.com/a/4233874
        public Dictionary<string, Func<RequestData, Response>> GetMethodHandler(Method method)
        {
            return method switch
            {
                Method.Get => GetHandler(),
                Method.Post => PostHandler(),
                Method.Put => PutHandler(),
                Method.Delete => DeleteHandler(),
                _ => ErrorHandler()
            };
        }

        private static Dictionary<string, Func<RequestData, Response>> GetHandler()
        {
            var getHandler =
                new Dictionary<string, Func<RequestData, Response>>
                {
                    { "/users", GetUser },
                    { "/cards", GetCards },
                    { "/score", GetScore },
                    { "/deck", GetDeck },
                    { "/stats", GetStats },
                    { "/tradings", GetTradings }
                };
            return getHandler;
        }

        private Dictionary<string, Func<RequestData, Response>> PostHandler()
        {
            var postHandler = new Dictionary<string, Func<RequestData, Response>>
            {
                { "/users", PostUser },
                { "/sessions", PostSessions },
                { "/packages", PostPackages },
                { "/transactions/packages", PostTransactionPackages },
                { "/battles", PostBattles },
                { "/tradings", PostTradings }
            };
            return postHandler;
        }

        private Dictionary<string, Func<RequestData, Response>> PutHandler()
        {
            var putHandler = new Dictionary<string, Func<RequestData, Response>>
            {
                { "/users", PutUser },
                { "/deck", PutDeck }
            };
            return putHandler;
        }

        private static Dictionary<string, Func<RequestData, Response>> DeleteHandler()
        {
            var putHandler = new Dictionary<string, Func<RequestData, Response>>
            {
                { "/tradings", DeleteTradings }
            };
            return putHandler;
        }

        private static Dictionary<string, Func<RequestData, Response>> ErrorHandler()
        {
            return new Dictionary<string, Func<RequestData, Response>> { { "/", ErrorMethod } };
        }

        private static Response ErrorMethod(RequestData data)
        {
            return Response.Status(Status.MethodNotAllowed);
        }
    }
}