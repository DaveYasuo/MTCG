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
        private readonly NpgsqlConnection _connection;
        private NpgsqlCommand Command { get; set; }
        public Db(IPrinter printer)
        {
            _printer = printer;
            string connString = ConnectionString.OnGet();
            _connection = new NpgsqlConnection(connString);
            _connection.Open();
        }

        public NpgsqlConnection GetConnection()
        {
            return _connection;
        }
        public void PrintVersion()
        {
            Command = new NpgsqlCommand();
            Command.Connection = _connection;
            const string sql = "SELECT version()";
            Command.CommandText = sql;
            string version = Command.ExecuteScalar()?.ToString();
            _printer.WriteLine($"PostgreSQL version: {version}");
        }

        ~Db()
        {
            _connection.Close();
        }
    }
}
