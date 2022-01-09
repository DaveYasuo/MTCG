using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using ServerModule.Database.Models;
using ServerModule.SimpleLogic.Mapping;
using ServerModule.SimpleLogic.Responses;
using ServerModule.Utility;

namespace ServerModule.SimpleLogic.Handler.RequestHandling
{
    public partial class RequestHandler
    {
        private static Response GetDeck(RequestData arg)
        {
            string username = arg.Authentication.Username;
            if (username is null) return Response.Status(Status.BadRequest);
            // Get all cards in deck, can be null, empty or not empty
            List<Card> cards = DataHandler.GetUserDeck(username, true);
            if (cards is null) return Response.Status(Status.InternalServerError);
            if (cards.Count == 0) return Response.PlainText("No cards in deck");
            // Check in which format it should be responded
            Dictionary<string, string> requestParams = arg.RequestParam;
            // Json format is default
            if (requestParams.Count == 0) return Response.Json(cards);
            // not supported format
            if (requestParams.Count != 1 || requestParams["format"] != "plain") return Response.Status(Status.BadRequest);
            // Using reflection
            // See: https://stackoverflow.com/a/19823887
            StringBuilder cardString = new StringBuilder();
            foreach (Card card in cards)
            {
                cardString.AppendLine(card.GetProperties());
            }
            return Response.PlainText(cardString.ToString());
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
            string username = arg.Authentication.Username;
            if (username is null) return Response.Status(Status.BadRequest);
            List<Card> cards = DataHandler.GetUserCards(username);
            return cards is null ? Response.Status(Status.NoContent) : Response.Json(cards);
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