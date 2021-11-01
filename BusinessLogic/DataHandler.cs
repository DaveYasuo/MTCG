using Data.Cards;
using Data.Users;
using DatabaseModule.PostgreSql;
using Npgsql;

namespace BusinessLogic
{
    class DataHandler
    {
        /**
         * Datenbank Abfrage wird hier durchgeführt. Die Connection sollte mithilfe des DatabaseModule hergestellt werden.
         * Alle Funktionen betreffend Abfrage wird hier eingefügt.
         **/
        private static NpgsqlCommand Command { get; set; }
        public static NpgsqlConnection Connection { get; set; }

        public DataHandler()
        {
            Connection = DB.GetConnection();
        }
        public static void InsertUser(User user)
        {
            Command = new NpgsqlCommand();
            Command.Connection = Connection;
            string sql = "INSERT INTO users(name,password) VALUES(@name,@password)";
            Command.CommandText = sql;
            Command.Parameters.AddWithValue("name", user.Username);
            Command.Parameters.AddWithValue("password", user.Password);
            Command.Prepare();
            Command.ExecuteNonQuery();
        }

        public static void InsertPackage(Package package)
        {
            Command = new NpgsqlCommand();
            Command.Connection = Connection;
            string sql = "INSERT INTO package(id,name,damage) VALUES(@id,@name,@damage)";
            Command.CommandText = sql;
            Command.Parameters.AddWithValue("id", package.Id);
            Command.Parameters.AddWithValue("name", package.Name);
            Command.Parameters.AddWithValue("damage", package.Damage);
            Command.Prepare();
            Command.ExecuteNonQuery();
        }
    }
}
