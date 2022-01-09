using System;
using System.Collections.Generic;
using System.Text.Json;
using ServerModule.Database.Models;
using ServerModule.Database.Schemas;
using ServerModule.SimpleLogic.Mapping;
using ServerModule.SimpleLogic.Responses;

namespace ServerModule.SimpleLogic.Handler.RequestHandling
{
    public partial class RequestHandler
    {
        private static Response PostTransactionPackages(RequestData requestData)
        {
            throw new NotImplementedException();
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
            string payload = data.Payload;
            // Get Json Array Length
            // See: https://docs.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement.getarraylength?view=net-6.0
            //if (payload?["array"] is not JsonElement { ValueKind: JsonValueKind.Array } cards || cards.GetArrayLength() is not 5) return Response.Status(Status.BadRequest);

            Credentials credentials = DataHandler.GetUser(data.Username);
            if (credentials == null) return Response.Status(Status.BadRequest);
            // double check: once with username from AuthorizationToken (request), and once from database (role). Second can happen when manually alter db-table
            if (data.Username.ToLower() != "admin" && credentials.Role.ToLower() != "admin")
                return Response.Status(Status.Forbidden);
            try
            {
                List<Card> package = JsonSerializer.Deserialize<List<Card>>(payload);
                if (package == null || package.Count != 5) return Response.Status(Status.BadRequest);
                //foreach (JsonElement card in cards.EnumerateArray())
                //{
                //    package.Add(new Card(card.GetProperty("Id").GetString(), card.GetProperty("Name").GetString(), card.GetProperty("Damage").GetDouble()));
                //}
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
                string token = user?.Login();
                return token != null
                    ? Response.PlainText("Welcome " + user.Username + Environment.NewLine + "Token: " + token)
                    : Response.PlainText("Invalid credentials", Status.Forbidden);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
            //User newUser = payload.GetObject<User>();
            try
            {
                User user = JsonSerializer.Deserialize<User>(payload);
                string token = user?.Register();
                return token != null
                    ? Response.PlainText("Register successful" + Environment.NewLine + "Token: " + token,
                        Status.Created)
                    : Response.PlainText("User already exists", Status.Conflict);
            }
            catch (Exception)
            {
                return Response.Status(Status.BadRequest);
            }
        }
    }
}