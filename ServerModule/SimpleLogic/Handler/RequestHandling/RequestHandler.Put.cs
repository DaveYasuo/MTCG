using System;
using System.Collections.Generic;
using System.Text.Json;
using DebugAndTrace;
using ServerModule.Database.Models;
using ServerModule.SimpleLogic.Mapping;
using ServerModule.SimpleLogic.Responses;

namespace ServerModule.SimpleLogic.Handler.RequestHandling
{
    public partial class RequestHandler
    {
        private static Response PutDeck(RequestData data)
        {
            string username = data.Authentication.Username;
            if (username is null) return Response.Status(Status.BadRequest);
            try
            {
                string payload = data.Payload;
                List<Guid> deck = JsonSerializer.Deserialize<List<Guid>>(payload);
                if (deck is not { Count: 4 }) return Response.PlainText("Wrong amount of cards selected", Status.BadRequest);
                return DataHandler.UpdateDeck(deck, username) ? Response.PlainText("Deck updated", Status.Created) : Response.PlainText("Failed to update deck", Status.BadRequest); ;
            }
            catch (Exception e)
            {
                Printer.Instance.WriteLine(e.Message);
                return Response.Status(Status.BadRequest);
            }
        }

        private static Response PutUser(RequestData requestData)
        {
            throw new NotImplementedException();
        }
    }
}