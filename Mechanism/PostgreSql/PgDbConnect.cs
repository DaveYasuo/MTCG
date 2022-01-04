using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Security.Authentication;
using DatabaseModule.Docker;
using Npgsql;

namespace DatabaseModule.PostgreSql
{
    public sealed class PgDbConnect : IPostgreSql
    {
        private NpgsqlConnection _connection;
        private static string _connString;

        private readonly string _host;
        private readonly string _username;
        private readonly string _password;
        private readonly string _database;


        /**
         * Default Constructor reads Credentials from Docker Environment Variables
         **/
        public PgDbConnect()
        {
            _host = "localhost";
            _username = DockerPostgresEnv.PostgresUser;
            _password = DockerPostgresEnv.PostgresPassword;
            _database = "postgres";
            Start();
        }

        public PgDbConnect(string host, string username, string password, string database)
        {
            _host = host;
            _username = username;
            _password = password;
            _database = database;
            Start();
        }

        ~PgDbConnect()
        {
            Stop();
        }

        public void Start()
        {
            Console.WriteLine("Hey");
            _connString = $"Host={_host};Username={_username};Password={_password};Database={_database};Include Error Detail=true;";
            try
            {
                // Check if _connString is valid, if not throws an exception -> Typing error
                _ = new NpgsqlConnectionStringBuilder(_connString);
                _connection = new NpgsqlConnection(_connString);
                // check if connection is valid, if not throws an exception -> host, username, password or database has Typing error
                _connection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                if (_connection.State != ConnectionState.Closed) _connection.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            _connection.Dispose();
        }

        public static NpgsqlConnection GetConnection()
        {
            if (_connString == null) _ = new PgDbConnect();
            NpgsqlConnection connection = new NpgsqlConnection(_connString);
            connection.Open();
            return connection;
        }

        public void PrintVersion()
        {
            using NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = _connection;
            const string sql = "SELECT version()";
            command.CommandText = sql;
            string version = command.ExecuteScalar()?.ToString();
            Console.WriteLine($"PostgreSQL version: {version}");
        }

        public void CheckDatabase()
        {

        }
    }
}
