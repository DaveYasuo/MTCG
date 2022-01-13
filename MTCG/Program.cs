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
            // To use the server, we must register some instances or templates to the container.
            Container container = new Container();
            container.RegisterInstance(Logger.GetPrinter(Printer.Debug));
            // For example the server needs a custom Security Handler that implements ISecurity
            // It doesn't need to be static or singleton because the server handles it, but doesn't not make a big difference
            container.Register<ISecurity, Security>();  //container.RegisterSingleton<ISecurity>(() => new Security());
            // the server also needs a custom RequestHandler that implements IRequestHandler
            container.Register<IRequestHandler, RequestHandler>();
            
            // Start Server
            //TcpListener listener = new TcpListener(10001);
            TcpServer server = new TcpServer(10001, container);
            server.Start();

        }
    }
}
