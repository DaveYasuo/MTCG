using System;
using ServerModule;

namespace MTCG.Database.PostgreSql
{
    public interface IPostgres : IServer, IDisposable
    {
        public void PrintVersion();
    }
}