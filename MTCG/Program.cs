using DebugAndTrace;
using ServerModule.Database.PostgreSql;
using ServerModule.SimpleLogic.Mapping;
using ServerModule.Tcp;
using ServerModule.Tcp.Listener;

namespace MTCG
{
    //docker stop swe1db

    //docker rm swe1db

    //docker run --name swe1db -e POSTGRES_USER=swe1user -e POSTGRES_PASSWORD=swe1pw -p 5432:5432 postgres

    internal class Program
    {
        private static void Main(string[] args)
        {
            // Initialize the output stream
            Printer.CreateInstance(ConsolePrinter.Instance);
            IPrinter printer = Printer.Instance;
            printer.WriteLine("Hello World!");
            //Db newDb = new Db(new ConsolePrinter(), "localhost", "test", "test", "test");
            PgDbConnect db = new PgDbConnect();
            db.PrintVersion();
            db.CreateDbIfNoExists();
            db.CreateTablesIfNoExist();
            db.Stop();
            //NpgsqlConnection connection = PgDbConnect.GetConnection();
            //new ConsolePrinter().WriteLine(new NpgsqlCommand("SELECT version()", connection).ExecuteScalar()?.ToString());

            Mapping mapping = new Mapping();
            TcpListener listener = new TcpListener(10001);
            TcpServer server = new TcpServer(listener, mapping);
            server.Start();


            printer.WriteLine("Hello Afterlife!");
        }
    }
}
