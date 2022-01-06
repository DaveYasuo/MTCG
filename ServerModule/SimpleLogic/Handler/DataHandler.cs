
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
            return PgDbConnect.GetConnection();
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

        public static UserSchema GetUser(string username)
        {
            using NpgsqlConnection conn = Connection();
            try
            {
                using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM player WHERE username=@p1", conn);
                cmd.Parameters.AddWithValue("p1", username);
                cmd.Prepare();
                using NpgsqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;
                UserSchema user = new UserSchema(

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

        public bool AddUser(UserSchema user)
        {

            const string sql = "INSERT INTO player(username,password,roleType) VALUES(@p1,@p2,@p3)";
            using NpgsqlConnection conn = Connection();
            using NpgsqlTransaction transaction = conn.BeginTransaction();
            try
            {
                using NpgsqlCommand command = new NpgsqlCommand(sql, conn, transaction);
                command.Parameters.AddWithValue("p1", user.Username);
                command.Parameters.AddWithValue("p2", user.Password);
                command.Parameters.AddWithValue("p3", user.RoleType);
                command.Prepare();
                _printer.WriteLine(command.ExecuteNonQuery());
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
