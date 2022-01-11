using DebugAndTrace;
using MTCG.BattleLogic;
using MTCG.Handler.RequestHandling;
using MTCG.Securities;
using ServerModule.Mapping;
using ServerModule.Tcp;
using ServerModule.Tcp.Listener;
using ServerModule.Security;

namespace MTCG
{
    //docker stop swe1db

    //docker rm swe1db

    //docker run --name swe1db -e POSTGRES_USER=swe1user -e POSTGRES_PASSWORD=swe1pw -p 5432:5432 postgres

    // test private code https://stackoverflow.com/questions/9122708/unit-testing-private-methods-in-c-sharp
    internal class Program
    {
        private static void Main()
        {
            // Initialize the output stream
            IPrinter log = Logger.GetPrinter(Printer.Debug);

            log.WriteLine("Hello World!");

            // start game service
            GameServer gameServer = new GameServer();

            // Initialize security data
            Security security = new Security();

            // Initialize Wrapper Class with security
            Authentication authentication = new Authentication(security);

            // Initialize RequestHandler
            RequestHandler requestHandler = new RequestHandler(gameServer, authentication, log);

            // Initialize Mapping for Routing
            Map map = new Map(requestHandler);

            // Start Server
            TcpListener listener = new TcpListener(10001);
            TcpServer server = new TcpServer(listener, map, authentication, log);
            server.Start();

            log.WriteLine("Hello Afterlife!");
        }
    }
}
