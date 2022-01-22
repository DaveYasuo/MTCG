using System.Collections.Generic;
using System.Text;
using MTCG.Database.Schemas;
using MTCG.Models;
using ServerModule.Mapping;
using ServerModule.Responses;
using ServerModule.Utility;

namespace MTCG.Handler.RequestHandling
{
    public partial class RequestHandler
    {
        /// <summary>
        /// Gets the current Deck of the user.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Returns the object (json or plaintext) if exists, else error response</returns>
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

        /// <summary>
        /// Gets the stats (win/loss/draw etc) from one user.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Returns the stats from the user as response, else error response</returns>
        private static Response GetStats(RequestData arg)
        {
            string username = arg.Authentication.Username;
            if (username is null) return Response.Status(Status.BadRequest);
            IStats stats = DataHandler.GetUserStats(username);
            return stats is null ? Response.Status(Status.BadRequest) : Response.Json(stats);
        }

        /// <summary>
        /// Gets the top player and user scoreboard from database.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Json response object where json can be empty, or server error response</returns>
        private static Response GetScore(RequestData arg)
        {
            string username = arg.Authentication.Username;
            if (username is null) return Response.Status(Status.BadRequest);
            List<Score> stats = DataHandler.GetUserScoreboard(username);
            return stats is null ? Response.Status(Status.InternalServerError) : Response.Json(stats);
        }

        /// <summary>
        /// Gets all acquired cards of the user.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Json Response or error response</returns>
        private static Response GetCards(RequestData arg)
        {
            string username = arg.Authentication.Username;
            if (username is null) return Response.Status(Status.BadRequest);
            List<Card> cards = DataHandler.GetUserCards(username);
            return cards is null ? Response.Status(Status.NoContent) : Response.Json(cards);
        }

        /// <summary>
        /// Get current User profile.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Returns profile as json response.</returns>
        private static Response GetUser(RequestData arg)
        {
            string authUsername = arg.Authentication.Username;
            string pathUsername = arg.PathVariable;
            if (authUsername is null || pathUsername is null) return Response.Status(Status.BadRequest);
            if (pathUsername != authUsername) return Response.Status(Status.Forbidden);
            IProfileData profile = DataHandler.GetProfileData(pathUsername);
            return profile is null ? Response.Status(Status.BadRequest) : Response.Json(profile);
        }

        /// <summary>
        /// Gets all current available trading deals
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Json response object with the trading deals or error response</returns>
        private static Response GetTradings(RequestData arg)
        {
            string user = arg.Authentication.Username;
            if (user is null) return Response.Status(Status.BadRequest);
            List<TradingDeal> tradingDeals = DataHandler.GetTradingDeals();
            if (tradingDeals == null) return Response.Status(500);
            return tradingDeals.Count == 0 ? Response.PlainText("No trading deals in bazaar") : Response.Json(tradingDeals);
        }
    }
}