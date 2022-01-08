
using System;
using System.Data;
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
    class DataHandler
    {
        private readonly IPrinter _printer = Printer.Instance;

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

                    reader.GetString(reader.GetOrdinal("username")),
                    reader.GetString(reader.GetOrdinal("password")),
                    reader.GetString(reader.GetOrdinal("role"))
                );
                return reader.Read() ? null : user;
            }
            catch (Exception)
            {
                return null;
            }
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
            const string credentials = "INSERT INTO credentials(username,password,role) VALUES(@p1,@p2,@p3)";
            const string profile = "INSERT INTO profile(username,name,bio,image,elo,wins,losses,draws,coins) VALUES(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9)";
            using NpgsqlConnection conn = Connection();
            using NpgsqlTransaction transaction = conn.BeginTransaction();
            try
            {
                using NpgsqlCommand credentialsCmd = new NpgsqlCommand(credentials, conn, transaction);
                credentialsCmd.Parameters.AddWithValue("p1", auth.Username);
                credentialsCmd.Parameters.AddWithValue("p2", auth.Password);
                credentialsCmd.Parameters.AddWithValue("p3", auth.Role);
                credentialsCmd.Prepare();

                Profile userProfile = new Profile(auth.Username);
                using NpgsqlCommand profileCmd = new NpgsqlCommand(profile, conn, transaction);
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
    }
}
