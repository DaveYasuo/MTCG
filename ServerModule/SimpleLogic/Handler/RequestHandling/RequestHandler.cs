using System;
using System.Collections.Generic;
using System.Text.Json;
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
    /**
     * Das ist die Logik.
     * Hier kommt eine switch-case.
     * Kommt drauf an, welche RawUrl, welche HTTP-Methode und welche Daten übergeben wurden.
     * Vlt eine Instanz der DataHandler erstellen und die Queries als Resultat wieder zurückgeben.
     **/
    public partial class RequestHandler
    {
        // Function in a dictionary
        // See: https://stackoverflow.com/a/30397975
        // and: https://stackoverflow.com/a/4233874
        public static Dictionary<string, Func<RequestData, Response>> GetMethodHandler(Method method)
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

        private static Dictionary<string, Func<RequestData, Response>> PostHandler()
        {
            Dictionary<string, Func<RequestData, Response>> postHandler = new Dictionary<string, Func<RequestData, Response>>
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
    }
}
