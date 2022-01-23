using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using MTCG.BattleLogic;
using MTCG.Data.Cards;
using MTCG.Data.Cards.Types;
using MTCG.Data.Users;

namespace MTCG_Test
{
    [TestFixture]
    public class BattleTest
    {
        [Test]
        public void BattlePlayer1Wins()
        {
            Mock<IMonsterCard> mock1 = new Mock<IMonsterCard>();
            mock1.Setup(card => card.MonsterType).Returns(MonsterType.Human);
            Mock<ICard> humanMock = mock1.As<ICard>();
            humanMock.Setup(card => card.Damage).Returns(100);
            ICard human = humanMock.Object;

            Mock<IMonsterCard> mock2 = new Mock<IMonsterCard>();
            mock2.Setup(card => card.MonsterType).Returns(MonsterType.Troll);
            Mock<ICard> trollMock = mock2.As<ICard>();
            trollMock.Setup(card => card.Damage).Returns(50);
            ICard troll = trollMock.Object;


            Mock<IPlayer> player1Mock = new Mock<IPlayer>();
            player1Mock.SetupGet(player => player.Username).Returns("Player1");
            player1Mock.SetupGet(player => player.Log).Returns(new List<object>());
            player1Mock.Setup(player => player.Cards).Returns(new List<ICard>());
            player1Mock.Setup(player => player.GetRandomCard()).Returns(human);
            IPlayer player1 = player1Mock.Object;

            Mock<IPlayer> player2Mock = new Mock<IPlayer>();
            player2Mock.SetupGet(player => player.Username).Returns("Player2");
            player2Mock.SetupGet(player => player.Log).Returns(new List<object>());
            player2Mock.Setup(player => player.Cards).Returns(new List<ICard>());
            player2Mock.Setup(player => player.GetRandomCard()).Returns(troll);
            IPlayer player2 = player2Mock.Object;

            Battle battle = new Battle(player1, player2);
            battle.StartGame();
            BattleResult result = battle.GetResult();
            Assert.IsFalse(result.Draw);
            Assert.AreEqual(player1.Username, result.Winner);
            Assert.AreEqual(player2.Username, result.Loser);
            Assert.Pass("Battle Player 1 wins");
        }

        [Test]
        public void BattlePlayer2Wins()
        {
            Mock<IMonsterCard> mock1 = new Mock<IMonsterCard>();
            mock1.Setup(card => card.MonsterType).Returns(MonsterType.Human);
            Mock<ICard> humanMock = mock1.As<ICard>();
            humanMock.Setup(card => card.Damage).Returns(50);
            ICard human = humanMock.Object;

            Mock<IMonsterCard> mock2 = new Mock<IMonsterCard>();
            mock2.Setup(card => card.MonsterType).Returns(MonsterType.Troll);
            Mock<ICard> trollMock = mock2.As<ICard>();
            trollMock.Setup(card => card.Damage).Returns(100);
            ICard troll = trollMock.Object;


            Mock<IPlayer> player1Mock = new Mock<IPlayer>();
            player1Mock.SetupGet(player => player.Username).Returns("Player1");
            player1Mock.SetupGet(player => player.Log).Returns(new List<object>());
            player1Mock.Setup(player => player.Cards).Returns(new List<ICard>());
            player1Mock.Setup(player => player.GetRandomCard()).Returns(human);
            IPlayer player1 = player1Mock.Object;

            Mock<IPlayer> player2Mock = new Mock<IPlayer>();
            player2Mock.SetupGet(player => player.Username).Returns("Player2");
            player2Mock.SetupGet(player => player.Log).Returns(new List<object>());
            player2Mock.Setup(player => player.Cards).Returns(new List<ICard>());
            player2Mock.Setup(player => player.GetRandomCard()).Returns(troll);
            IPlayer player2 = player2Mock.Object;

            Battle battle = new Battle(player1, player2);
            battle.StartGame();
            BattleResult result = battle.GetResult();
            Assert.IsFalse(result.Draw);
            Assert.AreEqual(player1.Username, result.Loser);
            Assert.AreEqual(player2.Username, result.Winner);
            Assert.Pass("Battle Player 2 wins");
        }

        [Test(Description = "Battle result is draw since both player have the same card")]
        public void BattleDrawGame()
        {
            Mock<IMonsterCard> mock1 = new Mock<IMonsterCard>();
            mock1.Setup(card => card.MonsterType).Returns(MonsterType.Human);
            Mock<ICard> humanMock = mock1.As<ICard>();
            humanMock.Setup(card => card.Damage).Returns(100);
            ICard human = humanMock.Object;
            
            Mock<IPlayer> player1Mock = new Mock<IPlayer>();
            player1Mock.SetupGet(player => player.Username).Returns("Player1");
            player1Mock.SetupGet(player => player.Log).Returns(new List<object>());
            player1Mock.Setup(player => player.Cards).Returns(new List<ICard>());
            player1Mock.Setup(player => player.GetRandomCard()).Returns(human);
            IPlayer player1 = player1Mock.Object;

            Mock<IPlayer> player2Mock = new Mock<IPlayer>();
            player2Mock.SetupGet(player => player.Username).Returns("Player2");
            player2Mock.SetupGet(player => player.Log).Returns(new List<object>());
            player2Mock.Setup(player => player.Cards).Returns(new List<ICard>());
            player2Mock.Setup(player => player.GetRandomCard()).Returns(human);
            IPlayer player2 = player2Mock.Object;

            Battle battle = new Battle(player1, player2);
            battle.StartGame();
            BattleResult result = battle.GetResult();
            Assert.IsTrue(result.Draw);
            Assert.Pass("Battle Draw Game");
        }
    }
}