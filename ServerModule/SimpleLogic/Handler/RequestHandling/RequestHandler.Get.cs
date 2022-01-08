using System;
using System.Collections.Generic;
using ServerModule.SimpleLogic.Mapping;
using ServerModule.SimpleLogic.Responses;

namespace ServerModule.SimpleLogic.Handler.RequestHandling
{
    public partial class RequestHandler
    {
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

        private static Response GetDeck(RequestData arg)
        {
            throw new NotImplementedException();
        }

        private static Response GetStats(RequestData arg)
        {
            throw new NotImplementedException();
        }

        private static Response GetScore(RequestData arg)
        {
            throw new NotImplementedException();
        }

        private static Response GetCards(RequestData arg)
        {
            throw new NotImplementedException();
        }

        private static Response GetUser(RequestData arg)
        {
            throw new NotImplementedException();
        }

        private static Response GetTradings(RequestData arg)
        {
            throw new NotImplementedException();
        }
    }
}