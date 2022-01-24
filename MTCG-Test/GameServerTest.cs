using DebugAndTrace;
using Moq;
using MTCG.BattleLogic;
using NUnit.Framework;

namespace MTCG_Test
{
    [TestFixture]
    public class GameServerTest
    {
        [SetUp]
        public void SetUp()
        {
            _server = new GameServer(new Mock<ILogger>().Object);
        }

        private GameServer _server;

        [Test]
        public void ServerStartStopTest()
        {
            _server.Start();
            _server.Stop();
            Assert.Pass("Starting and stopping game server happens without errors.");
        }

        [Test]
        public void ServerStopStartTest()
        {
            _server.Stop();
            _server.Start();
            Assert.Pass("Starting and stopping game server happens without errors.");
        }

        [Test]
        public void ServerMultipleStartsStopsTest()
        {
            _server.Start();
            _server.Start();
            _server.Stop();
            _server.Stop();
            _server.Start();
            _server.Stop();
            Assert.Pass("Starting and stopping game server happens without errors.");
        }
    }
}