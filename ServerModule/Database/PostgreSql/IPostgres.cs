using System;

namespace ServerModule.Database.PostgreSql
{
    public interface IPostgres : IServer, IDisposable
    {
        public void PrintVersion();
    }
}