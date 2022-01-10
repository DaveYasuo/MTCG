using System;
using System.Collections.Generic;
using System.Text.Json;
using ServerModule.BattleLogic;
using ServerModule.Database.Models;
using ServerModule.Database.Schemas;
using ServerModule.SimpleLogic.Mapping;
using ServerModule.SimpleLogic.Responses;
using ServerModule.Utility;

namespace ServerModule.SimpleLogic.Handler.RequestHandling
{
    // Partial classes are so useful
    // See: https://www.geeksforgeeks.org/partial-classes-in-c-sharp/
    // Naming convention
    // See: https://stackoverflow.com/questions/1478610/naming-conventions-for-partial-class-files
 public partial class RequestHandler
    {
        // Battle Logic
        private static GameServer _game = GameServer.Instance;
        // Function in a dictionary
        // See: https://stackoverflow.com/a/30397975
        // and: https://stackoverflow.com/a/4233874
        public static Dictionary<string, Func<RequestData, Response>> GetMethodHandler(Method method)
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
            Dictionary<string, Func<RequestData, Response>> getHandler =
                new Dictionary<string, Func<RequestData, Response>>
                {
                    { "/users", GetUser },
                    { "/cards", GetCards },
                    { "/score", GetScore },
                    { "/deck", GetDeck },
                    { "/stats", GetStats },
                    { "/tradings", GetTradings },
                };
            return getHandler;
        }

        private static Dictionary<string, Func<RequestData, Response>> PostHandler()
        {
            Dictionary<string, Func<RequestData, Response>> postHandler = new Dictionary<string, Func<RequestData, Response>>
            {
                { "/users", PostUser },
                { "/sessions", PostSessions},
                { "/packages", PostPackages},
                { "/transactions/packages", PostTransactionPackages},
                { "/battles", PostBattles },
                { "/tradings", PostTradings },
            };
            return postHandler;
        }

        private static Dictionary<string, Func<RequestData, Response>> PutHandler()
        {
            Dictionary<string, Func<RequestData, Response>> putHandler = new Dictionary<string, Func<RequestData, Response>>
            {
                { "/users", PutUser },
                { "/deck", PutDeck},
            };
            return putHandler;

        }

        private static Dictionary<string, Func<RequestData, Response>> DeleteHandler()
        {
            Dictionary<string, Func<RequestData, Response>> putHandler = new Dictionary<string, Func<RequestData, Response>>
            {
                {  "/tradings", DeleteTradings },
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
