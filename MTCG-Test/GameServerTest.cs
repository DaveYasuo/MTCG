using System.Collections.Generic;
using DebugAndTrace;
using Moq;
using MTCG.BattleLogic;
using MTCG.Models;
using NUnit.Framework;

namespace MTCG_Test
{
    [TestFixture]
    public class GameServerTest
    {
        private GameServer _server;

        [SetUp]
        public void SetUp()
        {
            _server = new GameServer(new Mock<ILogger>().Object);
        }

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