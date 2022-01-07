using System;

namespace ServerModule.Database.PostgreSql
{
    public interface IPostgreSql : IServer, IDisposable
    {
        public void PrintVersion();
    }
}