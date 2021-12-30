using System;
using System.Threading.Tasks;
using DatabaseModule.PostgreSql;
using DebugAndTrace;
using ServerModule.Tcp;

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
            Db newDb = new Db(new ConsolePrinter());
            newDb.PrintVersion();
            TcpServer server = new TcpServer(new ConsolePrinter(), 10001);
            server.Start();
            printer.WriteLine("Hello Afterlife!");
        }
    }
}
