using System;
using System.Collections.Generic;
using System.Text.Json;
using ServerModule.Database.Models;
using ServerModule.Database.Schemas;
using ServerModule.SimpleLogic.Responses;
using ServerModule.Utility;

namespace ServerModule.SimpleLogic.Handler
{
    public class RequestHandlerData
    {
        public RequestHandlerData(string username, object payload, string pathVariable, string requestParam)
        {
            RequestParam = requestParam;
            PathVariable = pathVariable;
            Payload = payload;
            Username = username;
        }

        public string Username { get; }
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
        public static Dictionary<string, Func<RequestHandlerData, Response>> GetMethodHandler(Method method)
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

        private static Dictionary<string, Func<RequestHandlerData, Response>> GetHandler()
        {
            Dictionary<string, Func<RequestHandlerData, Response>> getHandler = new Dictionary<string, Func<RequestHandlerData, Response>>
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

        private static Response GetDeck(RequestHandlerData arg)
        {
            throw new NotImplementedException();
        }

        private static Response GetStats(RequestHandlerData arg)
        {
            throw new NotImplementedException();
        }

        private static Response GetScore(RequestHandlerData arg)
        {
            throw new NotImplementedException();
        }

        private static Response GetCards(RequestHandlerData arg)
        {
            throw new NotImplementedException();
        }

        private static Response GetUser(RequestHandlerData arg)
        {
            throw new NotImplementedException();
        }

        private static Response GetTradings(RequestHandlerData arg)
        {
            throw new NotImplementedException();
        }

        private static Dictionary<string, Func<RequestHandlerData, Response>> PostHandler()
        {
            Dictionary<string, Func<RequestHandlerData, Response>> postHandler = new Dictionary<string, Func<RequestHandlerData, Response>>
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

        private static Response PostTransactionPackages(RequestHandlerData requestHandlerData)
        {
            throw new NotImplementedException();
        }

        private static Response PostTradings(RequestHandlerData requestHandlerData)
        {
            throw new NotImplementedException();
        }

        private static Response PostBattles(RequestHandlerData requestHandlerData)
        {
            throw new NotImplementedException();
        }

        private static Response PostTransaction(RequestHandlerData requestHandlerData)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Create packages
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static Response PostPackages(RequestHandlerData data)
        {
            Dictionary<string, object> payload = (Dictionary<string, object>)data.Payload;
            // Get Json Array Length
            // See: https://docs.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement.getarraylength?view=net-6.0
            if (payload?["array"] is not JsonElement { ValueKind: JsonValueKind.Array } cards || cards.GetArrayLength() is not 5) return Response.Status(Status.BadRequest);

            Credentials credentials = DataHandler.GetUser(data.Username);
            if (credentials == null) return Response.Status(Status.BadRequest);
            // double check: once with username from AuthorizationToken (request), and once from database (role). Second can happen when manually alter db-table
            if (data.Username.ToLower() != "admin" && credentials.Role.ToLower() != "admin") return Response.Status(Status.Forbidden);
            List<Card> package = new List<Card>();
            try
            {
                foreach (JsonElement card in cards.EnumerateArray())
                {
                    package.Add(new Card(card.GetProperty("Id").GetString(), card.GetProperty("Name").GetString(), card.GetProperty("Damage").GetDouble()));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Response.Status(Status.BadRequest);
            }
            return Response.Status(Status.BadRequest);
            //return token != null ? Response.PlainText("Welcome " + user.Username + Environment.NewLine + "Token: " + token) : Response.PlainText("Invalid credentials", Status.Forbidden);
        }
        /// <summary>
        /// Login User
        /// </summary>
        /// <param name="requestHandlerData"></param>
        /// <returns>Response object</returns>
        private static Response PostSessions(RequestHandlerData requestHandlerData)
        {
            Dictionary<string, object> payload = (Dictionary<string, object>)requestHandlerData.Payload;
            if (payload == null) return Response.Status(Status.BadRequest);
            User user = payload.GetObject<User>();
            string token = user.Login();
            return token != null ? Response.PlainText("Welcome " + user.Username + Environment.NewLine + "Token: " + token) : Response.PlainText("Invalid credentials", Status.Forbidden);
        }
        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="requestHandlerData"></param>
        /// <returns>Response object</returns>
        public static Response PostUser(RequestHandlerData requestHandlerData)
        {
            Dictionary<string, object> payload = (Dictionary<string, object>)requestHandlerData.Payload;
            if (payload == null) return Response.Status(Status.BadRequest);
            User newUser = payload.GetObject<User>();
            string token = newUser.Register();
            return token != null ? Response.PlainText("Register successful" + Environment.NewLine + "Token: " + token, Status.Created) : Response.PlainText("User already exists", Status.Conflict);
        }

        private static Dictionary<string, Func<RequestHandlerData, Response>> PutHandler()
        {
            Dictionary<string, Func<RequestHandlerData, Response>> putHandler = new Dictionary<string, Func<RequestHandlerData, Response>>
            {
                { "/users", PutUser },
                { "/deck", PutDeck},
            };
            return putHandler;

        }

        private static Response PutDeck(RequestHandlerData requestHandlerData)
        {
            throw new NotImplementedException();
        }

        private static Response PutUser(RequestHandlerData requestHandlerData)
        {
            throw new NotImplementedException();
        }

        private static Dictionary<string, Func<RequestHandlerData, Response>> DeleteHandler()
        {
            Dictionary<string, Func<RequestHandlerData, Response>> putHandler = new Dictionary<string, Func<RequestHandlerData, Response>>
            {
                {  "/tradings", DeleteTradings },
            };
            return putHandler;

        }

        private static Response DeleteTradings(RequestHandlerData requestHandlerData)
        {
            throw new NotImplementedException();
        }
    }
}
