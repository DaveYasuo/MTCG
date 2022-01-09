
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using DebugAndTrace;
using Npgsql;
using NpgsqlTypes;
using ServerModule.Database.Models;
using ServerModule.Database.PostgreSql;
using ServerModule.Database.Schemas;

namespace ServerModule.SimpleLogic.Handler
{
    /**
     * Datenbank Abfrage wird hier durchgeführt. Die Connection sollte mithilfe des pgDbConnect Klasse hergestellt werden.
     * Alle Funktionen betreffend Abfrage wird hier eingefügt.
     **/
    internal static class DataHandler
    {
        private static NpgsqlConnection Connection()
        {
            return Postgres.GetConnection();
        }
        // Basic usage of NpgSql
        // See: https://www.npgsql.org/doc/basic-usage.html
        // Usage of transactions
        // See: https://stackoverflow.com/a/55434778

        //public void InsertPackage(Package package)
        //{
        //    using NpgsqlCommand command = new NpgsqlCommand();
        //    command.Connection = Connection;
        //    const string sql = "INSERT INTO package(id,name,damage) VALUES(@id,@name,@damage)";
        //    command.CommandText = sql;
        //    command.Parameters.AddWithValue("id", package.Id);
        //    command.Parameters.AddWithValue("name", package.Username);
        //    command.Parameters.AddWithValue("damage", package.Damage);
        //    command.Prepare();
        //    command.ExecuteNonQuery();
        //}

        /// <summary>
        /// Get Credentials Object from database. Contains of id(token), username, password, role(admin/user)
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Returns null if failed, else Credentials object</returns>
        public static Credentials GetUser(string username)
        {
            using NpgsqlConnection conn = Connection();
            try
            {
                using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM credentials WHERE username=@p1", conn);
                cmd.Parameters.AddWithValue("p1", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;
                Credentials user = new Credentials(
                    reader.SafeGetString("token"),
                    reader.SafeGetString("username"),
                    reader.SafeGetString("password"),
                    reader.SafeGetString("role")
                );
                return reader.Read() ? null : user;
            }
            catch (Exception e)
            {
                Printer.Instance.WriteLine(e);
                return null;
            }
        }
        // Handling Null Data
        // See: https://stackoverflow.com/a/1772037
        public static string SafeGetString(this NpgsqlDataReader reader, string columnName)
        {
            int colIndex = reader.GetOrdinal(columnName);
            return !reader.IsDBNull(colIndex) ? reader.GetString(colIndex) : string.Empty;
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
            catch (Exception)
            {
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
            using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM credentials WHERE token=@p1", conn);
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
            catch (Exception e)
            {
                Console.WriteLine(e);
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
                using NpgsqlCommand cmd = new NpgsqlCommand("UPDATE credentials SET token=@p1 WHERE username=@p2", conn);
                cmd.Parameters.AddWithValue("p1", token);
                cmd.Parameters.AddWithValue("p2", username);
                cmd.Prepare();
                bool result = cmd.ExecuteNonQuery() != -1;
                transaction.Commit();
                return result;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}
