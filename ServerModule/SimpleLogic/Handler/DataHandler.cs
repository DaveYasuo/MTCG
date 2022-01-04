using Data.Cards;
using Data.Users;
using DatabaseModule.PostgreSql;
using DebugAndTrace;
using Npgsql;

namespace ServerModule.SimpleLogic.Handler
{
    class DataHandler
    {
        /**
         * Datenbank Abfrage wird hier durchgeführt. Die Connection sollte mithilfe des DatabaseModule hergestellt werden.
         * Alle Funktionen betreffend Abfrage wird hier eingefügt.
         **/
        public NpgsqlConnection Connection { get; set; }
        private readonly IPrinter _printer;

        public DataHandler(IPrinter printer)
        {
            _printer = printer;
            PgDbConnect pgDbConnect = new PgDbConnect();
            //Connection = pgDbConnect.GetConnection();
        }
        public void InsertUser(User user)
        {
            using NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = Connection;
            const string sql = "INSERT INTO users(name,password) VALUES(@name,@password)";
            command.CommandText = sql;
            command.Parameters.AddWithValue("name", user.Username);
            command.Parameters.AddWithValue("password", user.Password);
            command.Prepare();
            _printer.WriteLine(command.ExecuteNonQuery());
        }

        public void InsertPackage(Package package)
        {
            using NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = Connection;
            const string sql = "INSERT INTO package(id,name,damage) VALUES(@id,@name,@damage)";
            command.CommandText = sql;
            command.Parameters.AddWithValue("id", package.Id);
            command.Parameters.AddWithValue("name", package.Name);
            command.Parameters.AddWithValue("damage", package.Damage);
            command.Prepare();
            command.ExecuteNonQuery();
        }
    }
}
