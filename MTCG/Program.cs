using DatabaseModule.PostgreSql;
using DebugAndTrace;
using Npgsql;
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
            IPrinter printer = new ConsolePrinter();
            printer.WriteLine("Hello World!");

            //Db newDb = new Db(new ConsolePrinter(), "localhost", "test", "test", "test");
            //PgDbConnect newPgDbConnect = new PgDbConnect();
            //newPgDbConnect.PrintVersion();
            //newPgDbConnect.Stop();
            //NpgsqlConnection connection = PgDbConnect.GetConnection();
            //new ConsolePrinter().WriteLine(new NpgsqlCommand("SELECT version()", connection).ExecuteScalar()?.ToString());

            Mapping mapping = new Mapping();
            TcpListener listener = new TcpListener(10001);
            TcpServer server = new TcpServer(listener, new DebugPrinter(), mapping);
            server.Start();


            printer.WriteLine("Hello Afterlife!");
        }
    }
}
