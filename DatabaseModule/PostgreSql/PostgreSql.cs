using System;
using DebugAndTrace;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DatabaseModule.PostgreSql
{
    public class ConnectionString
    {
        public static string OnGet()
        {
            var config = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<ConnectionString>()
                .Build();
            return config.GetConnectionString("DefaultDB");
        }

    }
    public class Db
    {
        private readonly IPrinter _printer;
        private NpgsqlConnection Connection { get; }

        public Db(IPrinter printer)
        {
            _printer = printer;
            string connString = ConnectionString.OnGet();
            Connection = new NpgsqlConnection(connString);
            Connection.Open();
        }

        public NpgsqlConnection GetConnection()
        {
            return Connection;
        }
        public void PrintVersion()
        {
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = Connection;
            const string sql = "SELECT version()";
            command.CommandText = sql;
            string version = command.ExecuteScalar()?.ToString();
            _printer.WriteLine($"PostgreSQL version: {version}");
        }

        ~Db()
        {
            Connection.Close();
        }
    }
}
