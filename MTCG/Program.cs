using DebugAndTrace;
using MTCG.Handler.RequestHandling;
using MTCG.Securities;
using ServerModule.Requests;
using ServerModule.Security;
using ServerModule.Tcp;
using ServerModule.Utility;

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
            DependencyService.RegisterInstance(Logger.GetPrinter(Printer.Debug));
            // For example the server needs a custom Security Handler that implements ISecurity
            // It doesn't need to be static or singleton because the server handles it, so it does not make a difference
            DependencyService.Register<ISecurity, Security>();
            // the server also needs a custom RequestHandler that implements IRequestHandler
            DependencyService.Register<IRequestHandler, RequestHandler>();

            // Start Server
            var server = new TcpServer(10001);
            server.Start();
        }
    }
}