using System;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DatabaseModule.PostgreSql
{
    public class ConnectionString
    {
        public string OnGet()
        {
            var config = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<ConnectionString>()
                .Build();
            return config.GetConnectionString("DefaultDB");
        }

    }
    public class DB
    {
        private static readonly string Cs = new ConnectionString().OnGet();
        private static NpgsqlConnection Connection { get; }
        private static NpgsqlCommand Command { get; set; }
        static DB()
        {
            Connection = new NpgsqlConnection(Cs);
            Connection.Open();
            PrintVersion();
        }

        public static NpgsqlConnection GetConnection()
        {
            return Connection;
        }
        public static void PrintVersion()
        {
            Command = new NpgsqlCommand();
            Command.Connection = Connection;
            string sql = "SELECT version()";
            Command.CommandText = sql;
            string version = Command.ExecuteScalar()?.ToString();
            Console.WriteLine($"PostgreSQL version: {version}");
        }

        ~DB()
        {
            Connection.Close();
        }
    }
}
