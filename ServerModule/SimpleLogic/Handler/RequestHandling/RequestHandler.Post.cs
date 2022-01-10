using System;
using System.Collections.Generic;
using System.Text.Json;
using DebugAndTrace;
using ServerModule.Database.Models;
using ServerModule.Database.Schemas;
using ServerModule.SimpleLogic.Mapping;
using ServerModule.SimpleLogic.Responses;
using ServerModule.SimpleLogic.Security;

namespace ServerModule.SimpleLogic.Handler.RequestHandling
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

        private static Response PostBattles(RequestData requestData)
        {
            throw new NotImplementedException();
        }

        private static Response PostTransaction(RequestData requestData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create packages
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static Response PostPackages(RequestData data)
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
                return DataHandler.AddPackage(package) ? Response.PlainText("Package added", Status.Created) : Response.PlainText("Failed to create package", Status.BadRequest); ;
            }
            catch (Exception e)
            {
                Printer.Instance.WriteLine(e.Message);
                return Response.Status(Status.BadRequest);
            }
        }

        /// <summary>
        /// Login User
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns>Response object</returns>
        private static Response PostSessions(RequestData requestData)
        {
            string payload = requestData.Payload;
            if (payload == null) return Response.Status(Status.BadRequest);
            //User user = payload.GetObject<User>();
            try
            {
                User user = JsonSerializer.Deserialize<User>(payload);
                if (user == null) return Response.Status(Status.BadRequest);
                string token = user.Login();
                return token != null
                    ? Response.PlainText("Welcome " + user.Username + Environment.NewLine + "Token: " + token)
                    : Response.PlainText("Invalid credentials", Status.Forbidden);
            }
            catch (Exception e)
            {
                Printer.Instance.WriteLine(e.Message);
                return Response.PlainText("Invalid credentials", Status.Forbidden);
            }
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns>Response object</returns>
        public static Response PostUser(RequestData requestData)
        {
            string payload = requestData.Payload;
            if (payload == null) return Response.Status(Status.BadRequest);
            try
            {
                User user = JsonSerializer.Deserialize<User>(payload);
                if (user == null) return Response.Status(Status.BadRequest);
                return user.Register() ? Response.PlainText("Register successful", Status.Created) : Response.PlainText("User already exists", Status.Conflict);
            }
            catch (Exception e)
            {
                Printer.Instance.WriteLine(e.Message);
                return Response.Status(Status.BadRequest);
            }
        }
    }
}