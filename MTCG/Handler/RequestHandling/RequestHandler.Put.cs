using System;
using System.Collections.Generic;
using System.Text.Json;
using MTCG.Models;
using ServerModule.Mapping;
using ServerModule.Responses;

namespace MTCG.Handler.RequestHandling
{
    public partial class RequestHandler
    {
        /// <summary>
        ///     Updates Users current deck, else do nothing.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Returns Response success or failed.</returns>
        private Response PutDeck(RequestData data)
        {
            var username = data.Authentication.Username;
            if (username is null) return Response.Status(Status.BadRequest);
            var payload = data.Payload;
            if (string.IsNullOrEmpty(payload)) return Response.Status(Status.BadRequest);
            try
            {
                var deck = JsonSerializer.Deserialize<List<Guid>>(payload);
                if (deck is not { Count: 4 })
                    return Response.PlainText("Wrong amount of cards selected", Status.BadRequest);
                return DataHandler.UpdateDeck(deck, username)
                    ? Response.PlainText("Deck updated", Status.Created)
                    : Response.PlainText("Failed to update deck", Status.BadRequest);
            }
            catch (Exception e)
            {
                _log.WriteLine(e.Message);
                return Response.Status(Status.BadRequest);
            }
        }

        /// <summary>
        ///     Updates User Profile.
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns>If update is success or not.</returns>
        private static Response PutUser(RequestData requestData)
        {
            var authUsername = requestData.Authentication.Username;
            var pathUsername = requestData.PathVariable;
            if (authUsername is null || pathUsername is null) return Response.Status(Status.BadRequest);
            if (pathUsername != authUsername) return Response.Status(Status.Forbidden);
            var payload = requestData.Payload;
            if (string.IsNullOrEmpty(payload)) return Response.Status(Status.BadRequest);
            var data = JsonSerializer.Deserialize<UserData>(payload);
            return DataHandler.UpdateProfile(pathUsername, data)
                ? Response.PlainText("Updated profile success")
                : Response.Status(Status.InternalServerError);
        }
    }
}