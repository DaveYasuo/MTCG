using System;
using System.Threading.Tasks;
using DatabaseModule.PostgreSql;
using ServerModule.Tcp;

namespace MTCG
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            DB newDb = new DB();
            Task<TcpListen> listenTask = TcpListen.TcpTask();
            TcpListen newTcp = await listenTask;
            Console.WriteLine("Listen is ready");
        }
    }
}
