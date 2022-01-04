using System;
using System.Collections.Generic;
using Data.Users;
using ServerModule.SimpleLogic.Responses;
using ServerModule.Utility;

namespace ServerModule.SimpleLogic.Handler
{
    public class RequestHandlerData
    {
        public RequestHandlerData(string token, object payload, string pathVariable, string requestParam)
        {
            RequestParam = requestParam;
            PathVariable = pathVariable;
            Payload = payload;
            Token = token;
        }

        public string Token { get; }
        public object Payload { get; }
        public string PathVariable { get; }
        public string RequestParam { get; }
    }
    /**
     * Das ist die Logik.
     * Hier kommt eine switch-case.
     * Kommt drauf an, welche RawUrl, welche HTTP-Methode und welche Daten übergeben wurden.
     * Vlt eine Instanz der DataHandler erstellen und die Queries als Resultat wieder zurückgeben.
     **/
    public class RequestHandler
    {
        // Function in a dictionary
        // See: https://stackoverflow.com/a/30397975
        // and: https://stackoverflow.com/a/4233874
        public static Dictionary<string, Func<RequestHandlerData, object>> GetMethodHandler(Method method)
        {
            switch (method)
            {
                case Method.Get:
                    return GetHandler();
                case Method.Post:
                    return PostHandler();
                case Method.Put:
                    return PutHandler();
                case Method.Delete:
                    return DeleteHandler();
                default: return null;
            }
        }

        private static Dictionary<string, Func<RequestHandlerData, object>> GetHandler()
        {
            Dictionary<string, Func<RequestHandlerData, object>> getHandler = new Dictionary<string, Func<RequestHandlerData, object>>
            {
                { "/users", GetUser },
                { "/cards", GetCards},
                { "/score", GetScore},
                { "/deck", GetDeck},
                { "/stats", GetStats },
                { "/tradings", GetTradings },
            };
            return getHandler;
        }

        private static object GetDeck(RequestHandlerData arg)
        {
            throw new NotImplementedException();
        }

        private static object GetStats(RequestHandlerData arg)
        {
            throw new NotImplementedException();
        }

        private static object GetScore(RequestHandlerData arg)
        {
            throw new NotImplementedException();
        }

        private static object GetCards(RequestHandlerData arg)
        {
            throw new NotImplementedException();
        }

        private static object GetUser(RequestHandlerData arg)
        {
            throw new NotImplementedException();
        }

        private static object GetTradings(RequestHandlerData arg)
        {
            throw new NotImplementedException();
        }

        private static Dictionary<string, Func<RequestHandlerData, object>> PostHandler()
        {
            Dictionary<string, Func<RequestHandlerData, object>> postHandler = new Dictionary<string, Func<RequestHandlerData, object>>
            {
                { "/users", PostUser },
                { "/sessions", PostSessions},
                { "/packages", PostPackages},
                { "/transaction", PostTransaction},
                { "/transactions/packages", PostTransactionPackages},
                { "/battles", PostBattles },
                { "/tradings", PostTradings },
            };
            return postHandler;
        }

        private static object PostTransactionPackages(RequestHandlerData requestHandlerData)
        {
            throw new NotImplementedException();
        }

        private static object PostTradings(RequestHandlerData requestHandlerData)
        {
            throw new NotImplementedException();
        }

        private static object PostBattles(RequestHandlerData requestHandlerData)
        {
            throw new NotImplementedException();
        }

        private static object PostTransaction(RequestHandlerData requestHandlerData)
        {
            throw new NotImplementedException();
        }

        private static object PostPackages(RequestHandlerData requestHandlerData)
        {
            throw new NotImplementedException();
        }

        private static object PostSessions(RequestHandlerData requestHandlerData)
        {
            throw new NotImplementedException();
        }

        public static object PostUser(RequestHandlerData requestHandlerData)
        {
            Dictionary<string, object> payload = (Dictionary<string, object>)requestHandlerData.Payload;
            User newUser = payload.GetObject<User>();
            if (payload == null) return Response.Status(Status.BadRequest);
            var resultTuple = newUser.Register();
            var result = auth.Register((payload["Username"] as string)!, (payload["Password"] as string)!);
            return !result.Item1 ? Response.Status(Status.Conflict) : Response.PlainText(result.Item2, Status.Created);
        }

        private static Dictionary<string, Func<RequestHandlerData, object>> PutHandler()
        {
            Dictionary<string, Func<RequestHandlerData, object>> putHandler = new Dictionary<string, Func<RequestHandlerData, object>>
            {
                { "/users", PutUser },
                { "/deck", PutDeck},
            };
            return putHandler;

        }

        private static object PutDeck(RequestHandlerData requestHandlerData)
        {
            throw new NotImplementedException();
        }

        private static object PutUser(RequestHandlerData requestHandlerData)
        {
            throw new NotImplementedException();
        }

        private static Dictionary<string, Func<RequestHandlerData, object>> DeleteHandler()
        {
            Dictionary<string, Func<RequestHandlerData, object>> putHandler = new Dictionary<string, Func<RequestHandlerData, object>>
            {
                {  "/tradings", DeleteTradings },
            };
            return putHandler;

        }

        private static object DeleteTradings(RequestHandlerData requestHandlerData)
        {
            throw new NotImplementedException();
        }
    }
}
