using System;
using System.Data;
using System.Diagnostics;
using System.Net.Sockets;
using DebugAndTrace;
using Npgsql;
using ServerModule.Docker;

namespace ServerModule.Database.PostgreSql
{
    // Sealed = prevent inherit from this class
    // See: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/sealed
    public sealed class Postgres : IPostgres
    {
        private NpgsqlConnection _connection;
        private static string _connString;

        private readonly string _host;
        private readonly string _port;
        private readonly string _username;
        private readonly string _password;
        private readonly string _database;


        /// <summary>
        /// Default Constructor reads Credentials from Docker Environment Variables
        /// </summary>
        public Postgres()
        {
            _host = "host.docker.internal";
            _port = Environment.GetEnvironmentVariable(PgEnv.Port);
            _username = Environment.GetEnvironmentVariable(PgEnv.Username);
            _password = Environment.GetEnvironmentVariable(PgEnv.Password);
            _database = Environment.GetEnvironmentVariable(PgEnv.Database);
            Start();
        }

        public Postgres(string host, string port, string username, string password, string database)
        {
            _host = host;
            _port = port;
            _username = username;
            _password = password;
            _database = database;
            Start();
        }

        ~Postgres()
        {
            Dispose(false);
        }

        public void Start()
        {
            // Connection string parameters
            // See: https://www.npgsql.org/doc/connection-string-parameters.html
            //_connString = $"Host={_host};Port={_port};Username={_username};Password={_password};Include Error Detail=true;";
            _connString = $"Host={_host}; Port={_port}; Username={_username}; Password={_password};";
            try
            {
                // Check if Connection string is valid, if not throws an exception -> Typing error
                _ = new NpgsqlConnectionStringBuilder(_connString);
                // create connection
                _connection = new NpgsqlConnection(_connString);
                // check if connection is established, if not throws an exception -> host, username, password or database has Typing error
                _connection.Open();
                if (!ContainsDb()) CreateDb();
                CreateConnection();
                CreateTablesIfNoExist();
            }
            catch (Exception e)
            {
                Printer.Instance.WriteLine(e.Message);
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
                Postgres dbConnect = new Postgres();
                dbConnect.Stop();
            }
            NpgsqlConnection connection = new NpgsqlConnection(_connString);
            connection.Open();
            return connection;
        }

        private bool ContainsDb()
        {
            // Check if database exists
            // See: https://stackoverflow.com/a/20032567/12347616
            // ReSharper disable once StringLiteralTypo
            using NpgsqlCommand cmdCheck = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname='{_database}'", _connection);
            return cmdCheck.ExecuteScalar() != null;
        }

        private void CreateConnection()
        {
            // Close general connection and build new one to the database
            _connection.Close();
            // default connected database has the same name as username
            _connString += $"Database={_database};";
            _connection = new NpgsqlConnection(_connString);
            _connection.Open();
        }

        private void CreateDb()
        {
            // Database does not exist; Create database and tables 
            // See: https://stackoverflow.com/a/17840078/12347616
            using NpgsqlCommand cmd = new NpgsqlCommand($@"CREATE DATABASE {_database} ENCODING = 'UTF8'", _connection);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates 5 tables: cards, credentials, packages, profile and store if non exists.
        /// </summary>
        private void CreateTablesIfNoExist()
        {
            // Create Table
            // See: https://stackoverflow.com/a/35526607
            // Reserved keywords
            // See: https://www.postgresql.org/docs/current/sql-keywords-appendix.html
            // Data Types
            // See: https://www.postgresql.org/docs/9.1/datatype.html
            // And: https://www.postgresql.org/docs/9.1/datatype-numeric.html

            // order is important
            CreateCredentials();
            CreateProfile();
            CreatePackages();
            CreateCards();
            CreateStore();
            // Add constraint foreign key later on
            AlterCards();
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

        private void CreateCredentials()
        {
            // user is a reserved keyword
            // See: https://stackoverflow.com/a/22256451
            // Char Length for hashed Password SHA256
            // See: https://stackoverflow.com/a/247627
            // Unique tokens
            // See: https://www.postgresql.org/docs/9.2/ddl-constraints.html
            using var cmd = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS credentials(
                token VARCHAR(100),
                username VARCHAR(100) NOT NULL,
                password CHAR(64) NOT NULL,
                role VARCHAR(10) NOT NULL,
                PRIMARY KEY(username),
                UNIQUE (token)
            )", _connection);
            cmd.ExecuteNonQuery();
        }

        private void CreateProfile()
        {
            // why using 255 over 256?
            // See: https://stackoverflow.com/a/2340662
            // Usage of foreign keys
            // See: https://www.postgresqltutorial.com/postgresql-foreign-key/
            using var cmd = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS profile(
                username VARCHAR(100),
                name VARCHAR(100),
                bio VARCHAR(255),
                image VARCHAR(255),
                elo SMALLINT NOT NULL,
                wins INTEGER NOT NULL,
                losses INTEGER NOT NULL,
                draws INTEGER NOT NULL,
                coins BIGINT NOT NULL,
                PRIMARY KEY(username),
                CONSTRAINT fk_user
                    FOREIGN KEY(username)
                        REFERENCES credentials(username)
                        ON DELETE CASCADE
            )", _connection);
            cmd.ExecuteNonQuery();
        }

        private void CreatePackages()
        {
            // using UUID 
            // See: https://www.postgresql.org/docs/9.5/datatype.html
            using var cmd = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS packages(
                id UUID,
                PRIMARY KEY(id)
            )", _connection);
            cmd.ExecuteNonQuery();
        }

        private void CreateCards()
        {
            using var cmd = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS cards(
                id UUID,
                card_name VARCHAR(30) NOT NULL,
                damage REAL NOT NULL,
                package UUID,
                username VARCHAR(100),
                deck BOOLEAN,
                store UUID,
                PRIMARY KEY(id),
                CONSTRAINT fk_package
                    FOREIGN KEY(package)
                        REFERENCES packages(id)
                        ON DELETE SET NULL,
                CONSTRAINT fk_user
                    FOREIGN KEY(username)
                        REFERENCES profile(username)
                        ON DELETE CASCADE
            )", _connection);
            cmd.ExecuteNonQuery();
        }

        private void CreateStore()
        {
            using var cmd = new NpgsqlCommand(@"
            CREATE TABLE IF NOT EXISTS store(
                id UUID,
                card_id UUID,
                type VARCHAR(20),
                damage REAL,
                PRIMARY KEY(id),
                CONSTRAINT fk_card
                    FOREIGN KEY(card_id)
                        REFERENCES cards(id)
            )", _connection);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Add store.id constraint to card.store if constraint doesn't exists
        /// </summary>
        private void AlterCards()
        {
            // Check if constraint already exists
            // See: https://gist.github.com/vielhuber/bda4983431cd0ac7080b
            using var selectCmd = new NpgsqlCommand(@"
            SELECT COUNT(*)
            FROM information_schema.constraint_column_usage
            WHERE constraint_name = 'fk_store';
            ", _connection);
            object result = selectCmd.ExecuteScalar();
            if (result == null) return;
            if ((long)result == 0)
            {
                using var alterCommand = new NpgsqlCommand(@"
                    ALTER TABLE cards
                    ADD CONSTRAINT fk_store
                        FOREIGN KEY(store)
                            REFERENCES store(id)
                            ON DELETE SET NULL;
                    ", _connection);
                alterCommand.ExecuteNonQuery();
            }
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
