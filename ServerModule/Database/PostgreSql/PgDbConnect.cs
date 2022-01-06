using System;
using System.Data;
using System.Diagnostics;
using DebugAndTrace;
using Npgsql;
using ServerModule.Docker;

namespace ServerModule.Database.PostgreSql
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
            _database = DockerPostgresEnv.ContainerName;
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
            Dispose(false);
        }

        public void Start()
        {
            Console.WriteLine("Hey");
            _connString = $"Host={_host};Username={_username};Password={_password};Include Error Detail=true;";
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
            Dispose();
        }

        public static NpgsqlConnection GetConnection()
        {
            if (_connString == null)
            {
                PgDbConnect dbConnect = new PgDbConnect();
                dbConnect.Stop();
            }
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
            Printer.Instance.WriteLine($"PostgreSQL version: {version}");
        }

        public void CreateDbIfNoExists()
        {
            // Check if database exists
            // See: https://stackoverflow.com/a/20032567/12347616
            bool dbExists;
            // ReSharper disable once StringLiteralTypo
            using (NpgsqlCommand cmdCheck = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname='{_database}'", _connection))
            {
                dbExists = cmdCheck.ExecuteScalar() != null;
            }

            if (dbExists)
            {
                Console.WriteLine("DB Exists");
                // Close general connection and build new one to database
                _connection.Close();
                _connString += $"Database={_database};";
                _connection = new NpgsqlConnection(_connString);
                _connection.Open();
                return;
            }
            Console.WriteLine("DB !Exists");

            // Database does not exist; Create database and tables 
            // See: https://stackoverflow.com/a/17840078/12347616
            using (NpgsqlCommand cmd = new NpgsqlCommand($@"CREATE DATABASE {_database} ENCODING = 'UTF8'", _connection))
            {
                cmd.ExecuteNonQuery();
            }
            // Close general connection and build new one to database
            _connection.Close();
            _connString += $"Database={_database};";
            _connection = new NpgsqlConnection(_connString);
            _connection.Open();
        }

        public void CreateTablesIfNoExist()
        {
            // Create Table
            // See: https://stackoverflow.com/a/35526607
            // Varchar Length for hashed Password
            // See: https://stackoverflow.com/a/247627
            // user is a reserved keyword
            // See: https://stackoverflow.com/a/22256451
            using var cmd = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS player(
                username VARCHAR(100) NOT NULL,
                password VARCHAR(256) NOT NULL,
                roleType VARCHAR(10) NOT NULL,
                PRIMARY KEY(username)
            )", _connection);
            cmd.ExecuteNonQuery();
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
