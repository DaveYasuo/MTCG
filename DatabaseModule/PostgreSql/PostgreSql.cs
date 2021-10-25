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
            //string host = _config["Host"];
            //string username = _config["Username"];
            //string password = _config["Password"];
            //string database = _config["Database"];
            //string includeErrorDetail = _config["Include Error Detail"];
            //Console.WriteLine(config.GetConnectionString("DefaultDB"););
            //$"Host={host};Username={username};Password{password};Database={database};Include Error Detail={includeErrorDetail}";
            return config.GetConnectionString("DefaultDB");
        }

    }
    public class DB
    {
        private static readonly string Cs = new ConnectionString().OnGet();
        public static NpgsqlConnection Connection { get; }
        public static NpgsqlCommand Command { get; set; }
        static DB()
        {
            Connection = new NpgsqlConnection(Cs); ;
            Connection.Open();
            PrintVersion();
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
