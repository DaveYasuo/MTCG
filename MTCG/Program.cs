using DebugAndTrace;
using MTCG.Handler.RequestHandling;
using MTCG.Securities;
using ServerModule.Container;
using ServerModule.Mapping;
using ServerModule.Requests;
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
            IoCContainer container = new IoCContainer();
            container.RegisterInstance(Logger.GetPrinter(Printer.Debug));
            // Security should be singleton, but since we don't use it directly, it sufficient if we make the wrapper class singleton
            container.Register<ISecurity, Security>();  //container.RegisterSingleton<ISecurity>(() => new Security());
            // Wrapper Class for ISecurity
            container.RegisterSingleton<Authentication>();
            container.Register<IRequestHandler, RequestHandler>();
            container.Register<IMap, Map>();
            container.RegisterInstance<ITcpListener>(new TcpListener(10001));
            TcpServer server = container.GetInstance(typeof(TcpServer)) as TcpServer;

            // Initialize the output stream
            IPrinter log = container.GetInstance(typeof(IPrinter)) as IPrinter;

            if (log == null) return;
            log.WriteLine("Hello World!");

            // start game service
            //GameServer gameServer = new GameServer();

            // Initialize security data
            //Security security = new Security();

            // Initialize Wrapper Class with security
            //Authentication authentication = new Authentication(security);

            // Initialize RequestHandler
            //RequestHandler requestHandler = new RequestHandler(gameServer, authentication, log);

            // Initialize Mapping for Routing
            //Map map = new Map(requestHandler);

            // Start Server
            //TcpListener listener = new TcpListener(10001);
            //TcpServer server = new TcpServer(listener, map, authentication, log);
            server?.Start();

            log.WriteLine("Hello Afterlife!");
        }
    }
}
