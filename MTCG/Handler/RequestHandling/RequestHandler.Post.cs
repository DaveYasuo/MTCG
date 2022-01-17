using System;
using System.Collections.Generic;
using System.Text.Json;
using MTCG.Database.Schemas;
using MTCG.Models;
using ServerModule.Mapping;
using ServerModule.Responses;
using ServerModule.Security;

namespace MTCG.Handler.RequestHandling
{
    public partial class RequestHandler
    {
        private static Response PostTransactionPackages(RequestData requestData)
        {
            const int packageCost = 5;
            string username = requestData.Authentication.Username;
            long coins = DataHandler.GetUserCoins(username);
            if (coins is -1) return Response.Status(Status.InternalServerError);
            if (coins - packageCost >= 0)
                return DataHandler.AcquirePackage(username, packageCost) ? Response.PlainText("Package acquired", Status.Created) : Response.PlainText("Could not acquire package", Status.BadRequest);
            return Response.PlainText("Not enough coins", Status.BadRequest);
        }

        private static Response PostTradings(RequestData requestData)
        {
            throw new NotImplementedException();
        }

        private Response PostBattles(RequestData requestData)
        {
            string username = requestData.Authentication.Username;
            string token = requestData.Authentication.Token;
            if (username is null) return Response.Status(Status.BadRequest);
            // Get all cards in deck, can be null, empty or not empty
            List<Card> cards = DataHandler.GetUserDeck(username, true);
            if (cards is null) return Response.Status(Status.InternalServerError);
            if (cards.Count == 0) return Response.PlainText("Configure cards in deck first");
            if (!_auth.UpdateStatus(token, UserStatus.Occupied, UserStatus.Available)) return Response.PlainText("User already in game");
            List<string> result = _game.Play(username, cards);
            _auth.UpdateStatus(token, UserStatus.Available, UserStatus.Occupied);
            return result == null ? Response.Status(Status.InternalServerError) : Response.Json(result);
        }

        /// <summary>
        /// Create packages
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Response PostPackages(RequestData data)
        {
            Credentials credentials = DataHandler.GetCredentials(data.Authentication.Username);
            if (credentials == null) return Response.Status(Status.BadRequest);
            // Check if User has Admin Role
            // triple check: once with username from AuthorizationToken (request), and once from database (role). Second can happen when manually alter db-table.
            // Lastly check if Token from Request is equal to the token in the database
            if (!data.Authentication.Username.Equals("admin") || credentials.Role != Role.Admin || !data.Authentication.Token.Equals(credentials.Token)) return Response.Status(Status.Forbidden);
            try
            {
                string payload = data.Payload;
                List<Card> package = JsonSerializer.Deserialize<List<Card>>(payload);
                if (package is not { Count: 5 }) return Response.Status(Status.BadRequest);
                return DataHandler.AddPackage(package) ? Response.PlainText("Package added", Status.Created) : Response.PlainText("Failed to create package", Status.BadRequest);
            }
            catch (Exception e)
            {
                _log.WriteLine(e.Message);
                return Response.Status(Status.BadRequest);
            }
        }

        /// <summary>
        /// Login User
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns>Response object</returns>
        private Response PostSessions(RequestData requestData)
        {
            string payload = requestData.Payload;
            if (payload == null) return Response.Status(Status.BadRequest);
            //User user = payload.GetObject<User>();
            try
            {
                // since payload has all arguments that User-class has, we can just use the User object from Server for deserialization
                User user = JsonSerializer.Deserialize<User>(payload);
                if (user == null) return Response.Status(Status.BadRequest);
                user.StatusCode = UserStatus.Available;
                string token = _auth.Login(user);
                return token != null
                    ? Response.PlainText("Welcome " + user.Username + Environment.NewLine + "Token: " + token)
                    : Response.PlainText("Invalid credentials", Status.Forbidden);
            }
            catch (Exception e)
            {
                _log.WriteLine(e.Message);
                return Response.PlainText("Invalid credentials", Status.Forbidden);
            }
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns>Response object</returns>
        public Response PostUser(RequestData requestData)
        {
            string payload = requestData.Payload;
            if (payload == null) return Response.Status(Status.BadRequest);
            try
            {
                // since payload has all arguments that User-class has, we can just use the User object from Server for deserialization
                User user = JsonSerializer.Deserialize<User>(payload);
                if (user == null) return Response.Status(Status.BadRequest);
                return _auth.Register(user) ? Response.PlainText("Register successful", Status.Created) : Response.PlainText("User already exists", Status.Conflict);
            }
            catch (Exception e)
            {
                _log.WriteLine(e.Message);
                return Response.Status(Status.BadRequest);
            }
        }
    }
}