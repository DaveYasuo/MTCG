using System;
using System.Data;
using DebugAndTrace;
using MTCG.Docker;
using Npgsql;

namespace MTCG.Database.PostgreSql
{
    // Sealed = prevent inherit from this class
    // See: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/sealed
    public sealed class Postgres : IPostgres
    {
        private NpgsqlConnection _connection;

        public string ConnString { get; private set; }
        private readonly string _host;
        private readonly string _port;
        private readonly string _username;
        private readonly string _password;
        private readonly string _database;
        private readonly IPrinter _log;


        /// <summary>
        /// Default Constructor reads Credentials from Docker Environment Variables
        /// </summary>
        public Postgres(IPrinter log, bool autoDrop = false)
        {
            _host = "host.docker.internal";
            _port = Environment.GetEnvironmentVariable(PgEnv.Port);
            _username = Environment.GetEnvironmentVariable(PgEnv.Username);
            _password = Environment.GetEnvironmentVariable(PgEnv.Password);
            _database = Environment.GetEnvironmentVariable(PgEnv.Database);
            _log = log;
            Start();
            if (autoDrop) DropTable();
            CreateTablesIfNoExist();
        }

        public Postgres(string host, string port, string username, string password, string database, IPrinter log, bool autoDrop = false)
        {
            _host = host;
            _port = port;
            _username = username;
            _password = password;
            _database = database;
            _log = log;
            Start();
            if (autoDrop) DropTable();
            CreateTablesIfNoExist();
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
            ConnString = $"Host={_host}; Port={_port}; Username={_username}; Password={_password};";
            try
            {
                // Check if Connection string is valid, if not throws an exception -> Typing error
                _ = new NpgsqlConnectionStringBuilder(ConnString);
                // create connection
                _connection = new NpgsqlConnection(ConnString);
                // check if connection is established, if not throws an exception -> host, username, password or database has Typing error
                _connection.Open();
                if (!ContainsDb()) CreateDb();
                CreateConnection();
            }
            catch (Exception e)
            {
                _log.WriteLine(e.Message);
                throw;
            }
        }

        public void DropTable()
        {
            using var cmd = new NpgsqlCommand(@"
            alter table if exists cards
            drop constraint if exists fk_store;
            drop table if exists store;
            drop table if exists cards;
            drop table if exists profile;
            drop table if exists credentials;
            drop table if exists packages;
            ", _connection);
            cmd.ExecuteNonQuery();
        }

        public void Stop()
        {
            try
            {
                if (_connection.State != ConnectionState.Closed) _connection.Close();
            }
            catch (Exception e)
            {
                _log.WriteLine(e.Message);
            }
            Dispose();
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
            ConnString += $"Database={_database};";
            _connection = new NpgsqlConnection(ConnString);
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
            _log.WriteLine($"PostgreSQL version: {version}");
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
            if ((long)result != 0) return;
            using var alterCommand = new NpgsqlCommand(@"
                    ALTER TABLE cards
                    ADD CONSTRAINT fk_store
                        FOREIGN KEY(store)
                            REFERENCES store(id)
                            ON DELETE SET NULL;
                    ", _connection);
            alterCommand.ExecuteNonQuery();
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
