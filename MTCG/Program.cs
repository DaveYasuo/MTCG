﻿using DebugAndTrace;
using ServerModule.Database.PostgreSql;
using ServerModule.Docker;
using ServerModule.SimpleLogic.Mapping;
using ServerModule.Tcp;
using ServerModule.Tcp.Listener;
using ServerModule.Utility;

namespace MTCG
{
    //docker stop swe1db

    //docker rm swe1db

    //docker run --name swe1db -e POSTGRES_USER=swe1user -e POSTGRES_PASSWORD=swe1pw -p 5432:5432 postgres
    // test private code https://stackoverflow.com/questions/9122708/unit-testing-private-methods-in-c-sharp
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Initialize the output stream
            Printer.CreateInstance(ConsolePrinter.Instance);
            IPrinter printer = Printer.Instance;

            // Initialize Postgres Connection for faster performance (not required)
            using (new PgDbConnect()) { }

            printer.WriteLine("Hello World!");
            // Initialize Mapping for Routing
            Mapping mapping = new Mapping();
            // Start Server
            TcpListener listener = new TcpListener(10001);
            TcpServer server = new TcpServer(listener, mapping);
            server.Start();

            printer.WriteLine("Hello Afterlife!");
        }
    }
}
