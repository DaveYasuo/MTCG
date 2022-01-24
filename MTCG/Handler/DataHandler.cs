using System;
using System.Collections.Generic;
using System.Data;
using DebugAndTrace;
using MTCG.BattleLogic;
using MTCG.Database.PostgreSql;
using MTCG.Database.Schemas;
using Npgsql;
using MTCG.Models;

namespace MTCG.Handler
{
    public static class DataHandler
    {
        // Basic usage of NpgSql
        // See: https://www.npgsql.org/doc/basic-usage.html
        // Usage of transactions
        // See: https://stackoverflow.com/a/55434778

        private static readonly string ConnectionString;
        private static readonly ILogger Log = Logger.GetPrinter(Printer.Debug);

        static DataHandler()
        {
            Log.WriteLine("Starting DB");
            //using Postgres postgres = new Postgres(Log, true);
            using Postgres postgres =
                new Postgres("host.docker.internal", "5432", "swe1user", "swe1pw", "swe1db", Log, true);
            ConnectionString = postgres.ConnString;
            Log.WriteLine("DB started");
        }

        private static NpgsqlConnection Connection()
        {
            NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        // Handling Null Data
        // See: https://stackoverflow.com/a/1772037
        // And: https://stackoverflow.com/a/52204396
        public static T SafeGet<T>(this NpgsqlDataReader reader, string columnName)
        {
            return reader.IsDBNull(columnName) ? default : reader.GetFieldValue<T>(columnName);
        }

        /// <summary>
        /// Add the given Credentials object to the database
        /// </summary>
        /// <param name="auth"></param>
        /// <returns>Returns true if success, else false</returns>
        public static bool AddUser(Credentials auth)
        {
            // two sql statements but one transaction
            // See: https://stackoverflow.com/a/175138
            const string credentialsSql = "INSERT INTO credentials (username,password,role) VALUES (@p1,@p2,@p3)";
            const string profileSql =
                "INSERT INTO profile (username,name,bio,image,elo,wins,losses,draws,coins) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9)";
            Profile userProfile = new Profile(auth.Username);
            using NpgsqlConnection conn = Connection();
            using NpgsqlTransaction transaction = conn.BeginTransaction();
            try
            {
                // Using Batching
                // See: https://www.npgsql.org/doc/basic-usage.html#batching
                using NpgsqlBatch cmd = new NpgsqlBatch(conn, transaction)
                {
                    BatchCommands =
                    {
                        new NpgsqlBatchCommand(credentialsSql)
                        {
                            Parameters =
                            {
                                new NpgsqlParameter("p1", auth.Username),
                                new NpgsqlParameter("p2", auth.Password),
                                new NpgsqlParameter("p3", auth.Role.ToString())
                            }
                        },
                        new NpgsqlBatchCommand(profileSql)
                        {
                            Parameters =
                            {
                                new NpgsqlParameter("p1", userProfile.Username),
                                new NpgsqlParameter("p2", userProfile.Name),
                                new NpgsqlParameter("p3", userProfile.Bio),
                                new NpgsqlParameter("p4", userProfile.Image),
                                new NpgsqlParameter("p5", userProfile.Elo),
                                new NpgsqlParameter("p6", userProfile.Wins),
                                new NpgsqlParameter("p7", userProfile.Losses),
                                new NpgsqlParameter("p8", userProfile.Draws),
                                new NpgsqlParameter("p9", userProfile.Coins)
                            }
                        }
                    }
                };
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                transaction.Rollback();
                return false;
            }
        }

        /// <summary>
        /// Check if Token exists in the database.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Return true if exists, else false.</returns>
        public static bool ContainsToken(string token)
        {
            using NpgsqlConnection conn = Connection();
            using NpgsqlCommand cmd = new NpgsqlCommand("SELECT token FROM credentials WHERE token=@p1", conn);
            cmd.Parameters.AddWithValue("p1", token);
            cmd.Prepare();
            return cmd.ExecuteScalar() != null;
        }

        /// <summary>
        /// Adds the cards into the cards table with the guid package id and also insert the guid into the package table.
        /// </summary>
        /// <param name="cards"></param>
        /// <returns>True on success, else false.</returns>
        public static bool AddPackage(List<Card> cards)
        {
            using NpgsqlConnection conn = Connection();
            using NpgsqlTransaction transaction = conn.BeginTransaction();
            try
            {
                // Generate new package id with Guid
                Guid packageId = Guid.NewGuid();
                const string packageSql = "INSERT INTO packages VALUES (@p)";
                using (NpgsqlCommand packageCmd = new NpgsqlCommand(packageSql, conn, transaction))
                {
                    packageCmd.Parameters.AddWithValue("p", packageId);
                    packageCmd.Prepare();
                    packageCmd.ExecuteNonQuery();
                }

                // i think for better performance
                // See: https://github.com/npgsql/npgsql/issues/2779
                // fixed issue wrong type
                // See: https://www.npgsql.org/doc/types/basic.html
                using (NpgsqlBinaryImporter importer =
                    conn.BeginBinaryImport(
                        "COPY cards (id, card_name, damage, package, deck) FROM STDIN (FORMAT binary)"))
                {
                    foreach (var card in cards)
                    {
                        importer.StartRow();
                        importer.Write(card.Id);
                        importer.Write(card.Name);
                        importer.Write(card.Damage);
                        importer.Write(packageId);
                        importer.Write(false);
                    }

                    importer.Complete();
                }

                transaction.Commit();
                return true;
            }
            catch (NpgsqlException e)
            {
                // catching duplicate key value violates unique constraint (error code 23505) and rollback
                if (e.SqlState != "23505") Log.WriteLine(e.Message);
                transaction.Rollback();
                return false;
            }
            catch (Exception e)
            {
                // unknown error
                Log.WriteLine(e.Message);
                transaction.Rollback();
                return false;
            }
        }

        /// <summary>
        /// Insert or updates the Token of a specific user.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="username"></param>
        /// <returns>True on success, else false.</returns>
        public static bool AddTokenToUser(string token, string username)
        {
            using NpgsqlConnection conn = Connection();
            using NpgsqlTransaction transaction = conn.BeginTransaction();
            try
            {
                int affectedRows;
                using (NpgsqlCommand cmd = new NpgsqlCommand("UPDATE credentials SET token=@p1 WHERE username=@p2",
                    conn, transaction))
                {
                    cmd.Parameters.AddWithValue("p1", token);
                    cmd.Parameters.AddWithValue("p2", username);
                    cmd.Prepare();
                    affectedRows = cmd.ExecuteNonQuery();
                }

                if (affectedRows != 0 && affectedRows != -1)
                {
                    transaction.Commit();
                    return true;
                }

                transaction.Rollback();
                return false;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                transaction.Rollback();
                return false;
            }
        }

        /// <summary>
        /// Get Credentials Object from database. Contains of id(token), username, password, role(admin/user)
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Returns null if failed, else Credentials object</returns>
        public static Credentials GetCredentials(string username)
        {
            using NpgsqlConnection conn = Connection();
            try
            {
                using NpgsqlCommand cmd =
                    new NpgsqlCommand("SELECT token, username, password, role FROM credentials WHERE username=@p1",
                        conn);
                cmd.Parameters.AddWithValue("p1", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;
                Credentials user = new Credentials(
                    reader.SafeGet<string>("token"),
                    reader.SafeGet<string>("username"),
                    reader.SafeGet<string>("password"),
                    reader.SafeGet<string>("role")
                );
                return reader.Read() ? null : user;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Get Profile object from one user in the database. Contains of username, name, bio, image, elo, wins, losses, draws, coins.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Returns the profile object on success, else null</returns>
        public static Profile GetUserProfile(string username)
        {
            using NpgsqlConnection conn = Connection();
            try
            {
                using NpgsqlCommand cmd = new NpgsqlCommand(
                    "SELECT username, name, bio, image, elo, wins, losses, draws, coins FROM profile WHERE username=@p1",
                    conn);
                cmd.Parameters.AddWithValue("p1", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;
                Profile user = new Profile(
                    reader.SafeGet<string>("username"),
                    reader.SafeGet<string>("name"),
                    reader.SafeGet<string>("bio"),
                    reader.SafeGet<string>("image"),
                    reader.SafeGet<short>("elo"),
                    reader.SafeGet<int>("wins"),
                    reader.SafeGet<int>("losses"),
                    reader.SafeGet<int>("draws"),
                    reader.SafeGet<long>("coins")
                );
                return reader.Read() ? null : user;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Get coins of a specific user from the database
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Returns the coins or -1 if an error occurred</returns>
        public static long GetUserCoins(string username)
        {
            // Why I use extra function for getting only coins, instead of getting whole user profile
            // See: https://stackoverflow.com/a/67380
            using NpgsqlConnection conn = Connection();
            try
            {
                using NpgsqlCommand cmd = new NpgsqlCommand("SELECT coins FROM profile WHERE username=@p1", conn);
                cmd.Parameters.AddWithValue("p1", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read()) return -1;
                long coins = reader.SafeGet<long>("coins");
                return reader.Read() ? -1 : coins;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                return -1;
            }
        }

        /// <summary>
        /// Add username to the cards, if he/she can afford.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="packageCost"></param>
        /// <returns>True, if package acquired, else false if not enough coins, no packages left or error </returns>
        public static bool AcquirePackage(string username, int packageCost)
        {
            // Check again if user has enough coins
            long coins = GetUserCoins(username);
            if (coins is -1 || coins - packageCost < 0)
            {
                return false;
            }

            using NpgsqlConnection conn = Connection();
            using NpgsqlTransaction transaction = conn.BeginTransaction();
            try
            {
                bool result;
                const string coinsSql = "UPDATE profile SET coins=@p1 WHERE username=@p2";
                using (NpgsqlCommand profileCmd = new NpgsqlCommand(coinsSql, conn, transaction))
                {
                    profileCmd.Parameters.AddWithValue("p1", coins - packageCost);
                    profileCmd.Parameters.AddWithValue("p2", username);
                    profileCmd.Prepare();
                    result = profileCmd.ExecuteNonQuery() == 1;
                }

                if (!result) return false;
                object pkIdResult;
                // Select first created package 
                // See: https://kb.objectrocket.com/postgresql/how-to-use-the-postgres-to-select-first-record-1271
                using (NpgsqlCommand packageIdCmd =
                    new NpgsqlCommand("SELECT id from packages LIMIT 1", conn, transaction))
                {
                    packageIdCmd.Prepare();
                    pkIdResult = packageIdCmd.ExecuteScalar();
                }

                if (pkIdResult == null)
                {
                    transaction.Rollback();
                    return false;
                }

                Guid packageId = (Guid)pkIdResult;

                // Update all cards with the associated package id: add username to the cards
                using (NpgsqlCommand cardCmd = new NpgsqlCommand("UPDATE cards SET username=@p1 WHERE package=@p2",
                    conn, transaction))
                {
                    cardCmd.Parameters.AddWithValue("p1", username);
                    cardCmd.Parameters.AddWithValue("p2", packageId);
                    cardCmd.Prepare();
                    if (cardCmd.ExecuteNonQuery() != 5)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

                // Delete package now
                using (NpgsqlCommand packageDeleteCmd =
                    new NpgsqlCommand("DELETE FROM packages WHERE id=@p1", conn, transaction))
                {
                    packageDeleteCmd.Parameters.AddWithValue("p1", packageId);
                    if (packageDeleteCmd.ExecuteNonQuery() == 1)
                    {
                        transaction.Commit();
                        return true;
                    }
                }

                transaction.Rollback();
                return false;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                transaction.Rollback();
                return false;
            }
        }

        /// <summary>
        /// Retrieves all acquired cards from the specified user.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>A List of the cards or null if error occurred</returns>
        public static List<Card> GetUserCards(string username)
        {
            using NpgsqlConnection conn = Connection();
            try
            {
                List<Card> cards = new List<Card>();
                using NpgsqlCommand cmd =
                    new NpgsqlCommand("SELECT id, card_name, damage FROM cards WHERE username=@p1", conn);
                cmd.Parameters.AddWithValue("p1", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Card card = new Card(
                        reader.SafeGet<Guid>("id"),
                        reader.SafeGet<string>("card_name"),
                        reader.SafeGet<float>("damage")
                    );
                    cards.Add(card);
                }

                return cards;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Query db to get user's deck if true, else get all cards that are not in deck
        /// </summary>
        /// <param name="username"></param>
        /// <param name="inDeck"></param>
        /// <returns>Returns the queried Cards as a List, or null if failed</returns>
        public static List<Card> GetUserDeck(string username, bool inDeck)
        {
            using NpgsqlConnection conn = Connection();
            try
            {
                List<Card> cards = new List<Card>();
                using NpgsqlCommand cmd =
                    new NpgsqlCommand("SELECT id, card_name, damage FROM cards WHERE username=@p1 AND deck=@p2", conn);
                cmd.Parameters.AddWithValue("p1", username);
                cmd.Parameters.AddWithValue("p2", inDeck);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Card card = new Card(
                        reader.SafeGet<Guid>("id"),
                        reader.SafeGet<string>("card_name"),
                        reader.SafeGet<float>("damage")
                    );
                    cards.Add(card);
                }

                return cards;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Set cards of user back to default (false in deck) and then updates the cards in the list to true in deck.
        /// </summary>
        /// <param name="deck"></param>
        /// <param name="username"></param>
        /// <returns>Returns true on success, else false.</returns>
        public static bool UpdateDeck(List<Guid> deck, string username)
        {
            using NpgsqlConnection conn = Connection();
            using NpgsqlTransaction transaction = conn.BeginTransaction();
            try
            {
                // Check if card is in store
                foreach (Guid cardId in deck)
                {
                    using NpgsqlCommand checkCard = new NpgsqlCommand("SELECT id FROM store WHERE card_id=@p1", conn);
                    checkCard.Parameters.AddWithValue("p1", cardId);
                    checkCard.Prepare();
                    if (checkCard.ExecuteScalar() == null) continue;
                    transaction.Rollback();
                    return false;
                }

                // Set all deck cards of user to false
                using (NpgsqlCommand setFalseCmd =
                    new NpgsqlCommand("UPDATE cards SET deck=@p1 WHERE username=@p2 AND deck=@p3", conn, transaction))
                {
                    setFalseCmd.Parameters.AddWithValue("p1", false);
                    setFalseCmd.Parameters.AddWithValue("p2", username);
                    setFalseCmd.Parameters.AddWithValue("p3", true);
                    setFalseCmd.Prepare();
                    setFalseCmd.ExecuteNonQuery();
                }

                // now set the given cards to true
                foreach (Guid cardId in deck)
                {
                    using NpgsqlCommand setTrueCmd =
                        new NpgsqlCommand("UPDATE cards SET deck=@p1 WHERE username=@p2 AND id=@p3", conn, transaction);
                    setTrueCmd.Parameters.AddWithValue("p1", true);
                    setTrueCmd.Parameters.AddWithValue("p2", username);
                    setTrueCmd.Parameters.AddWithValue("p3", cardId);
                    setTrueCmd.Prepare();
                    // if no or more than one row is affected means, the user doesn't own the card or an error occurred
                    if (setTrueCmd.ExecuteNonQuery() == 1) continue;
                    transaction.Rollback();
                    return false;
                }

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                transaction.Rollback();
                return false;
            }
        }

        /// <summary>
        /// Set new profile data to the user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userData"></param>
        /// <returns>True on success, else false</returns>
        public static bool UpdateProfile(string username, UserData userData)
        {
            using NpgsqlConnection conn = Connection();
            using NpgsqlTransaction transaction = conn.BeginTransaction();
            try
            {
                using NpgsqlCommand cmd =
                    new NpgsqlCommand("UPDATE profile SET name=@p1, bio=@p2, image=@p3 WHERE username=@p4", conn,
                        transaction);
                cmd.Parameters.AddWithValue("p1", userData.Name);
                cmd.Parameters.AddWithValue("p2", userData.Bio);
                cmd.Parameters.AddWithValue("p3", userData.Image);
                cmd.Parameters.AddWithValue("p4", username);
                cmd.Prepare();
                if (cmd.ExecuteNonQuery() == 1)
                {
                    transaction.Commit();
                    return true;
                }

                transaction.Rollback();
                return false;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                transaction.Rollback();
                return false;
            }
        }

        /// <summary>
        /// Get ProfileData object from one user in the database. Contains of username, name, bio, image.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Returns the profile data object on success, else null</returns>
        public static IProfileData GetProfileData(string username)
        {
            using NpgsqlConnection conn = Connection();
            try
            {
                using NpgsqlCommand cmd =
                    new NpgsqlCommand("SELECT username, name, bio, image FROM profile WHERE username=@p1", conn);
                cmd.Parameters.AddWithValue("p1", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;
                IProfileData user = new ProfileData(
                    reader.SafeGet<string>("username"),
                    reader.SafeGet<string>("name"),
                    reader.SafeGet<string>("bio"),
                    reader.SafeGet<string>("image")
                );
                return reader.Read() ? null : user;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Get Profile object from user in the database. Contains of username, elo, wins, losses, draws, coins.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Returns the profile object on success, else null</returns>
        public static IStats GetUserStats(string username)
        {
            using NpgsqlConnection conn = Connection();
            try
            {
                using NpgsqlCommand cmd =
                    new NpgsqlCommand(
                        "SELECT username, elo, wins, losses, draws, coins FROM profile WHERE username=@p1", conn);
                cmd.Parameters.AddWithValue("p1", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;
                IStats user = new ProfileStats(
                    reader.SafeGet<string>("username"),
                    reader.SafeGet<short>("elo"),
                    reader.SafeGet<int>("wins"),
                    reader.SafeGet<int>("losses"),
                    reader.SafeGet<int>("draws"),
                    reader.SafeGet<long>("coins")
                );
                return reader.Read() ? null : user;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Get the top player as a List and also add the surrounding player of user at the end of the list, if user is not on the top
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Returns a List of IStats, if error returns null</returns>
        public static List<Score> GetUserScoreboard(string username)
        {
            // if not in top 10 of Scoreboard, show player before and after user too
            // See: https://www.the-art-of-web.com/sql/select-before-after/
            using NpgsqlConnection conn = Connection();
            try
            {
                List<Score> scoreList = new List<Score>();
                using (NpgsqlCommand cmd = new NpgsqlCommand(
                    "SELECT row_number() OVER (ORDER BY elo DESC), username, elo, wins, losses, draws FROM profile LIMIT 10;",
                    conn))
                {
                    cmd.Parameters.AddWithValue("p1", username);
                    cmd.Prepare();
                    using NpgsqlDataReader reader = cmd.ExecuteReader();
                    bool isInList = false;
                    while (reader.Read())
                    {
                        Score playerStats = new Score(
                            reader.SafeGet<long>("row_number"),
                            reader.SafeGet<string>("username"),
                            reader.SafeGet<short>("elo"),
                            reader.SafeGet<int>("wins"),
                            reader.SafeGet<int>("losses"),
                            reader.SafeGet<int>("draws")
                        );
                        scoreList.Add(playerStats);
                        if (playerStats.Username == username) isInList = true;
                    }

                    if (isInList) return scoreList;
                }

                using (NpgsqlCommand cmd = new NpgsqlCommand(@"
                WITH cte AS (SELECT row_number() OVER (ORDER BY elo DESC), username, elo, wins, losses, draws FROM profile),
                     current AS (SELECT row_number FROM cte WHERE username =@p1)
                SELECT cte.* FROM cte, current WHERE ABS(cte.row_number - current.row_number) <= 2 ORDER BY cte.row_number;",
                    conn))
                {
                    cmd.Parameters.AddWithValue("p1", username);
                    cmd.Prepare();
                    using NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Score playerStats = new Score(
                            reader.SafeGet<long>("row_number"),
                            reader.SafeGet<string>("username"),
                            reader.SafeGet<short>("elo"),
                            reader.SafeGet<int>("wins"),
                            reader.SafeGet<int>("losses"),
                            reader.SafeGet<int>("draws")
                        );
                        scoreList.Add(playerStats);
                    }

                    return scoreList;
                }
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Updates UserStats with the results of the game.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="winCoins"></param>
        /// <param name="lossCoins"></param>
        /// <param name="drawCoins"></param>
        /// <param name="addWinElo"></param>
        /// <param name="addLossElo"></param>
        /// <returns>True if update was successful, else false.</returns>
        public static bool UpdateGameResult(BattleResult result, string player1, string player2, int winCoins = 3,
            int lossCoins = 1, int drawCoins = 2, int addWinElo = 3, int addLossElo = -5)
        {

            using NpgsqlConnection conn = Connection();
            using NpgsqlTransaction transaction = conn.BeginTransaction();
            if (result.Draw)
            {
                try
                {
                    // increment value
                    // See: https://stackoverflow.com/a/10233360
                    using NpgsqlCommand cmd = new NpgsqlCommand(
                        "UPDATE profile SET draws=draws+1, coins=coins+@p1 WHERE username=@p2 OR username=@p3 ", conn,
                        transaction);
                    cmd.Parameters.AddWithValue("p1", drawCoins);
                    cmd.Parameters.AddWithValue("p2", player1);
                    cmd.Parameters.AddWithValue("p3", player2);
                    cmd.Prepare();
                    if (cmd.ExecuteNonQuery() == 2)
                    {
                        transaction.Commit();
                        return true;
                    }

                    transaction.Rollback();
                    return false;
                }
                catch (Exception e)
                {
                    Log.WriteLine(e.Message);
                    transaction.Rollback();
                    return false;
                }
            }

            string loser = result.Winner == player1 ? player2 : player1;
            try
            {
                using NpgsqlBatch cmd = new NpgsqlBatch(conn, transaction)
                {
                    BatchCommands =
                    {
                        new NpgsqlBatchCommand(
                            "UPDATE profile SET elo=elo+@p1, wins=wins+1, coins=coins+@p2 WHERE username=@p3")
                        {
                            Parameters =
                            {
                                new NpgsqlParameter("p1", addWinElo),
                                new NpgsqlParameter("p2", winCoins),
                                new NpgsqlParameter("p3", result.Winner)
                            }
                        },
                        new NpgsqlBatchCommand(
                            "UPDATE profile SET elo=elo+@p1, losses=losses+1, coins=coins+@p2 WHERE username=@p3")
                        {
                            Parameters =
                            {
                                new NpgsqlParameter("p1", addLossElo),
                                new NpgsqlParameter("p2", lossCoins),
                                new NpgsqlParameter("p3", loser)
                            }
                        }
                    }
                };
                cmd.Prepare();
                if (cmd.ExecuteNonQuery() == 2)
                {
                    transaction.Commit();
                    return true;
                }

                transaction.Rollback();
                return false;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                transaction.Rollback();
                return false;
            }
        }

        /// <summary>
        /// Get all trading deals from the database
        /// </summary>
        /// <returns>A list of deals which can be empty or null on error</returns>
        public static List<TradingDeal> GetTradingDeals()
        {

            using NpgsqlConnection conn = Connection();
            List<TradingDeal> store = new List<TradingDeal>();
            try
            {
                using NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, card_id, type, damage FROM store", conn);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    TradingDeal deal = new TradingDeal(
                        reader.SafeGet<Guid>("id"),
                        reader.SafeGet<Guid>("card_id"),
                        reader.SafeGet<string>("type"),
                        reader.SafeGet<float>("damage")
                    );
                    store.Add(deal);
                }

                return store;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Adds a card to the store and remove it from deck
        /// </summary>
        /// <param name="username"></param>
        /// <param name="tradingDeal"></param>
        /// <returns>True on success, else false.</returns>
        public static bool AddTradingDeal(string username, TradingDeal tradingDeal)
        {
            using NpgsqlConnection conn = Connection();
            using NpgsqlTransaction transaction = conn.BeginTransaction();
            try
            {
                const string dealSql = "INSERT INTO store (id,card_id,type,damage) VALUES (@p1,@p2,@p3,@p4)";
                const string deckSql = "UPDATE cards SET deck=@p1, store=@p2 WHERE id=@p3 AND username=@p4";
                using NpgsqlBatch cmd = new NpgsqlBatch(conn, transaction)
                {
                    BatchCommands =
                    {
                        new NpgsqlBatchCommand(dealSql)
                        {
                            Parameters =
                            {
                                new NpgsqlParameter("p1", tradingDeal.Id),
                                new NpgsqlParameter("p2", tradingDeal.CardToTrade),
                                new NpgsqlParameter("p3", tradingDeal.Type),
                                new NpgsqlParameter("p4", tradingDeal.MinimumDamage)
                            }
                        },
                        new NpgsqlBatchCommand(deckSql)
                        {
                            Parameters =
                            {
                                new NpgsqlParameter("p1", false),
                                new NpgsqlParameter("p2", tradingDeal.Id),
                                new NpgsqlParameter("p3", tradingDeal.CardToTrade),
                                new NpgsqlParameter("p4", username)
                            }
                        }
                    }
                };
                cmd.Prepare();
                if (cmd.ExecuteNonQuery() == 2)
                {
                    transaction.Commit();
                    return true;
                }

                transaction.Rollback();
                return false;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                transaction.Rollback();
                return false;
            }
        }

        /// <summary>
        /// Delete accepted card from store and add it to one's own stack
        /// </summary>
        /// <param name="dealId"></param>
        /// <param name="dealCard"></param>
        /// <param name="tradeCard"></param>
        /// <returns>True, if trade was a success, else false</returns>
        public static bool AcceptTradingDeal(Guid dealId, CardWithUsername dealCard, CardWithUsername tradeCard)
        {
            using NpgsqlConnection conn = Connection();
            using NpgsqlTransaction transaction = conn.BeginTransaction();
            try
            {
                const string deleteSql = "DELETE FROM store WHERE id=@p1";
                const string updateDealCardSql = "UPDATE cards SET username=@p1 WHERE id=@p2";
                const string updateTradeCardSql = "UPDATE cards SET deck=@p1, username=@p2 WHERE id=@p3";
                using NpgsqlBatch cmd = new NpgsqlBatch(conn, transaction)
                {
                    BatchCommands =
                    {
                        new NpgsqlBatchCommand(deleteSql)
                        {
                            Parameters =
                            {
                                new NpgsqlParameter("p1", dealId)
                            }
                        },
                        new NpgsqlBatchCommand(updateDealCardSql)
                        {
                            Parameters =
                            {
                                new NpgsqlParameter("p1", tradeCard.Username),
                                new NpgsqlParameter("p2", dealCard.Id)
                            }
                        },
                        new NpgsqlBatchCommand(updateTradeCardSql)
                        {
                            Parameters =
                            {
                                new NpgsqlParameter("p1", false),
                                new NpgsqlParameter("p2", dealCard.Username),
                                new NpgsqlParameter("p3", tradeCard.Id)
                            }
                        }
                    }
                };
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                transaction.Rollback();
                return false;
            }
        }

        /// <summary>
        /// Get the trading card with the specific id from store.
        /// </summary>
        /// <returns>A card on success or null, if no cards are in store</returns>
        public static TradingDeal GetCardOfStore(string id)
        {
            Guid idGuid = Guid.Parse(id);
            using NpgsqlConnection conn = Connection();
            try
            {
                using NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, card_id, type, damage FROM store WHERE id=@p1", conn);
                cmd.Parameters.AddWithValue("p1", idGuid);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;
                TradingDeal card = new TradingDeal(
                    reader.SafeGet<Guid>("id"),
                    reader.SafeGet<Guid>("card_id"),
                    reader.SafeGet<string>("type"),
                    reader.SafeGet<float>("damage")
                );
                return reader.Read() ? null : card;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Get the card with the specific Guid id from cards.
        /// </summary>
        /// <returns>A card on success or null, if an error occurred.</returns>
        public static CardWithUsername GetCard(Guid id)
        {
            using NpgsqlConnection conn = Connection();
            try
            {
                using NpgsqlCommand cmd =
                    new NpgsqlCommand("SELECT id, card_name, damage, username FROM cards WHERE id=@p1", conn);
                cmd.Parameters.AddWithValue("p1", id);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;
                CardWithUsername card = new CardWithUsername(
                    reader.SafeGet<Guid>("id"),
                    reader.SafeGet<string>("card_name"),
                    reader.SafeGet<float>("damage"),
                    reader.SafeGet<string>("username")
                );
                return reader.Read() ? null : card;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Get the card with the specific string id from cards.
        /// </summary>
        /// <returns>A card on success or null, if an error occurred.</returns>
        public static CardWithUsername GetCard(string id)
        {
            Guid idGuid = Guid.Parse(id);
            using NpgsqlConnection conn = Connection();
            try
            {
                using NpgsqlCommand cmd =
                    new NpgsqlCommand("SELECT id, card_name, damage, username FROM cards WHERE id=@p1", conn);
                cmd.Parameters.AddWithValue("p1", idGuid);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;
                CardWithUsername card = new CardWithUsername(
                    reader.SafeGet<Guid>("id"),
                    reader.SafeGet<string>("card_name"),
                    reader.SafeGet<float>("damage"),
                    reader.SafeGet<string>("username")
                );
                return reader.Read() ? null : card;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Deletes card with the given ID from the store.
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns>True on success, else false</returns>
        public static bool DeleteTradingDeal(string storeId)
        {
            Guid idGuid = Guid.Parse(storeId);
            using NpgsqlConnection conn = Connection();
            using NpgsqlTransaction transaction = conn.BeginTransaction();
            try
            {
                using NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM store WHERE id=@p1", conn, transaction);
                cmd.Parameters.AddWithValue("p1", idGuid);
                cmd.Prepare();
                if (cmd.ExecuteNonQuery() == 1)
                {
                    transaction.Commit();
                    return true;
                }

                transaction.Rollback();
                return false;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                transaction.Rollback();
                return false;
            }
        }
    }
}
