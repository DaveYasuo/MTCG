using System;
using System.Collections.Generic;
using System.Data;
using DebugAndTrace;
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
        private static readonly IPrinter Log = Logger.GetPrinter(Printer.Debug);

        static DataHandler()
        {
            Log.WriteLine("Starting DB");
            //Postgres postgres = new Postgres(Log, true);
            Postgres postgres = new Postgres("host.docker.internal", "5432", "swe1user", "swe1pw", "swe1db", Log, true);
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
            const string profileSql = "INSERT INTO profile (username,name,bio,image,elo,wins,losses,draws,coins) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9)";
            using NpgsqlConnection conn = Connection();
            using NpgsqlTransaction transaction = conn.BeginTransaction();
            try
            {
                using NpgsqlCommand credentialsCmd = new NpgsqlCommand(credentialsSql, conn, transaction);
                credentialsCmd.Parameters.AddWithValue("p1", auth.Username);
                credentialsCmd.Parameters.AddWithValue("p2", auth.Password);
                credentialsCmd.Parameters.AddWithValue("p3", auth.Role.ToString());
                credentialsCmd.Prepare();

                Profile userProfile = new Profile(auth.Username);
                using NpgsqlCommand profileCmd = new NpgsqlCommand(profileSql, conn, transaction);
                profileCmd.Parameters.AddWithValue("p1", userProfile.Username);
                profileCmd.Parameters.AddWithValue("p2", userProfile.Name);
                profileCmd.Parameters.AddWithValue("p3", userProfile.Bio);
                profileCmd.Parameters.AddWithValue("p4", userProfile.Image);
                profileCmd.Parameters.AddWithValue("p5", userProfile.Elo);
                profileCmd.Parameters.AddWithValue("p6", userProfile.Wins);
                profileCmd.Parameters.AddWithValue("p7", userProfile.Losses);
                profileCmd.Parameters.AddWithValue("p8", userProfile.Draws);
                profileCmd.Parameters.AddWithValue("p9", userProfile.Coins);
                profileCmd.Prepare();

                credentialsCmd.ExecuteNonQuery();
                profileCmd.ExecuteNonQuery();
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
                using var packageCmd = new NpgsqlCommand(packageSql, conn, transaction);
                packageCmd.Parameters.AddWithValue("p", packageId);
                packageCmd.Prepare();
                packageCmd.ExecuteNonQuery();

                foreach (var card in cards)
                {
                    const string cardSql = "INSERT INTO cards (id, card_name, damage, package, deck) VALUES (@p1, @p2, @p3, @p4, @p5)";
                    using var cardCmd = new NpgsqlCommand(cardSql, conn, transaction);
                    cardCmd.Parameters.AddWithValue("p1", card.Id);
                    cardCmd.Parameters.AddWithValue("p2", card.Name);
                    cardCmd.Parameters.AddWithValue("p3", card.Damage);
                    cardCmd.Parameters.AddWithValue("p4", packageId);
                    cardCmd.Parameters.AddWithValue("p5", false);
                    cardCmd.ExecuteNonQuery();
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
                using NpgsqlCommand cmd = new NpgsqlCommand("UPDATE credentials SET token=@p1 WHERE username=@p2", conn, transaction);
                cmd.Parameters.AddWithValue("p1", token);
                cmd.Parameters.AddWithValue("p2", username);
                cmd.Prepare();
                int affectedRows = cmd.ExecuteNonQuery();
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
                using NpgsqlCommand cmd = new NpgsqlCommand("SELECT token, username, password, role FROM credentials WHERE username=@p1", conn);
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
                using NpgsqlCommand cmd = new NpgsqlCommand("SELECT username, name, bio, image, elo, wins, losses, draws, coins FROM profile WHERE username=@p1", conn);
                cmd.Parameters.AddWithValue("p1", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;
                Profile user = new Profile(
                    reader.SafeGet<string>("username"),
                    reader.SafeGet<string>("name"),
                    reader.SafeGet<string>("bio"),
                    reader.SafeGet<string>("image"),
                    reader.SafeGet<int>("elo"),
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
                const string coinsSql = "UPDATE profile SET coins=@p1 WHERE username=@p2";
                using NpgsqlCommand profileCmd = new NpgsqlCommand(coinsSql, conn, transaction);
                profileCmd.Parameters.AddWithValue("p1", coins - packageCost);
                profileCmd.Parameters.AddWithValue("p2", username);
                profileCmd.Prepare();
                bool result = profileCmd.ExecuteNonQuery() == 1;
                if (!result) return false;

                // Select first created package 
                // See: https://kb.objectrocket.com/postgresql/how-to-use-the-postgres-to-select-first-record-1271
                using var packageIdCmd = new NpgsqlCommand("SELECT id from packages LIMIT 1", conn, transaction);
                packageIdCmd.Prepare();
                object pkIdResult = packageIdCmd.ExecuteScalar();
                if (pkIdResult == null)
                {
                    transaction.Rollback();
                    return false;
                }
                Guid packageId = (Guid)pkIdResult;

                // Update all cards with the associated package id: add username to the cards
                using var cardCmd = new NpgsqlCommand("UPDATE cards SET username=@p1 WHERE package=@p2", conn, transaction);
                cardCmd.Parameters.AddWithValue("p1", username);
                cardCmd.Parameters.AddWithValue("p2", packageId);
                cardCmd.Prepare();
                if (cardCmd.ExecuteNonQuery() != 5)
                {
                    transaction.Rollback();
                    return false;
                }

                // Delete package now
                using var packageDeleteCmd = new NpgsqlCommand("DELETE FROM packages WHERE id=@p1", conn, transaction);
                packageDeleteCmd.Parameters.AddWithValue("p1", packageId);
                if (packageDeleteCmd.ExecuteNonQuery() == 1)
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
                using NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, card_name, damage FROM cards WHERE username=@p1", conn);
                cmd.Parameters.AddWithValue("p1", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Card card = new Card(
                        reader.SafeGet<Guid>("id"),
                        reader.SafeGet<string>("card_name"),
                        reader.SafeGet<double>("damage")
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
                using NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, card_name, damage FROM cards WHERE username=@p1 AND deck=@p2", conn);
                cmd.Parameters.AddWithValue("p1", username);
                cmd.Parameters.AddWithValue("p2", inDeck);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Card card = new Card(
                        reader.SafeGet<Guid>("id"),
                        reader.SafeGet<string>("card_name"),
                        reader.SafeGet<double>("damage")
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
                using NpgsqlCommand setFalseCmd = new NpgsqlCommand("UPDATE cards SET deck=@p1 WHERE username=@p2 AND deck=@p3", conn, transaction);
                setFalseCmd.Parameters.AddWithValue("p1", false);
                setFalseCmd.Parameters.AddWithValue("p2", username);
                setFalseCmd.Parameters.AddWithValue("p3", true);
                setFalseCmd.Prepare();
                setFalseCmd.ExecuteNonQuery();

                // now set the given cards to true
                foreach (Guid cardId in deck)
                {
                    using NpgsqlCommand setTrueCmd = new NpgsqlCommand("UPDATE cards SET deck=@p1 WHERE username=@p2 AND id=@p3", conn, transaction);
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
                using NpgsqlCommand cmd = new NpgsqlCommand("UPDATE profile SET name=@p1, bio=@p2, image=@p3 WHERE username=@p4", conn, transaction);
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
                using NpgsqlCommand cmd = new NpgsqlCommand("SELECT username, name, bio, image FROM profile WHERE username=@p1", conn);
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
                using NpgsqlCommand cmd = new NpgsqlCommand("SELECT username, elo, wins, losses, draws, coins FROM profile WHERE username=@p1", conn);
                cmd.Parameters.AddWithValue("p1", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;
                IStats user = new ProfileStats(
                    reader.SafeGet<string>("username"),
                    reader.SafeGet<int>("elo"),
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
                using NpgsqlCommand cmd = new NpgsqlCommand("SELECT row_number() OVER (ORDER BY elo), username, elo, wins, losses, draws FROM profile LIMIT 10;", conn);
                cmd.Parameters.AddWithValue("p1", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                bool isInList = false;
                while (reader.Read())
                {
                    Score playerStats = new Score(
                        reader.SafeGet<long>("row_number"),
                        reader.SafeGet<string>("username"),
                        reader.SafeGet<int>("elo"),
                        reader.SafeGet<int>("wins"),
                        reader.SafeGet<int>("losses"),
                        reader.SafeGet<int>("draws")
                    );
                    scoreList.Add(playerStats);
                    if (playerStats.Username == username) isInList = true;
                }
                if (isInList) return scoreList;
                using NpgsqlCommand cmd1 = new NpgsqlCommand(@"
                WITH cte AS (SELECT row_number() OVER (ORDER BY elo), username, elo, wins, losses, draws FROM profile),
                     current AS (SELECT row_number FROM cte WHERE username =@p1)
                SELECT cte.* FROM cte, current WHERE ABS(cte.row_number - current.row_number) <= 2 ORDER BY cte.row_number;", conn);
                cmd1.Parameters.AddWithValue("p1", username);
                cmd1.Prepare();
                using NpgsqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    Score playerStats = new Score(
                        reader.SafeGet<long>("row_number"),
                        reader.SafeGet<string>("username"),
                        reader.SafeGet<int>("elo"),
                        reader.SafeGet<int>("wins"),
                        reader.SafeGet<int>("losses"),
                        reader.SafeGet<int>("draws")
                    );
                    scoreList.Add(playerStats);
                }
                return scoreList;
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                return null;
            }
        }
    }
}
