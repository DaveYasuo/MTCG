using Data.Cards;
using Data.Users;
using DatabaseModule.PostgreSql;
using DebugAndTrace;
using Npgsql;

namespace BusinessLogic
{
    class DataHandler
    {
        /**
         * Datenbank Abfrage wird hier durchgeführt. Die Connection sollte mithilfe des DatabaseModule hergestellt werden.
         * Alle Funktionen betreffend Abfrage wird hier eingefügt.
         **/
        private NpgsqlCommand Command { get; set; }
        public NpgsqlConnection Connection { get; set; }
        private IPrinter Printer { get; set; }

        public DataHandler(IPrinter printer)
        {
            Printer = printer;
            Db newDb = new Db(printer);
            Connection = newDb.GetConnection();
        }
        public void InsertUser(User user)
        {
            Command = new NpgsqlCommand();
            Command.Connection = Connection;
            string sql = "INSERT INTO users(name,password) VALUES(@name,@password)";
            Command.CommandText = sql;
            Command.Parameters.AddWithValue("name", user.Username);
            Command.Parameters.AddWithValue("password", user.Password);
            Command.Prepare();
            Printer.WriteLine(Command.ExecuteNonQuery());
        }

        public void InsertPackage(Package package)
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
