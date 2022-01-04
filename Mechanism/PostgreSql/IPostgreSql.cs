using Npgsql;

namespace DatabaseModule.PostgreSql
{
    public interface IPostgreSql : IServer
    {
        public void PrintVersion();
        public void CheckDatabase();
    }
}