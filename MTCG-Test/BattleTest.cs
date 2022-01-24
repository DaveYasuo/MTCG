using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Moq;
using MTCG.BattleLogic;
using MTCG.Data.Cards;
using MTCG.Data.Cards.Monster;
using MTCG.Data.Cards.Specialties;
using MTCG.Data.Cards.Spell;
using MTCG.Data.Cards.Types;
using MTCG.Data.Users;

namespace MTCG_Test
{
    [TestFixture]
    public class BattleTest
    {
        [Test]
        public void BattleOneRoundPlayer1Wins()
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
        public void BattleOneRoundPlayer2Wins()
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
        public void BattleOneRoundDrawGame()
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

        [Test(Description =
            "Battle result can not draw even though there is a bonus round every ten rounds and fifty percent chance to deal double damage."
            + " Player2's WoodKnight's max damage is 80 and Player1's FireHuman's min damage is 100.")]
        public void BattleUntilPlayerOneWins()
        {
            Mock<IMonsterCard> mock1 = new Mock<IMonsterCard>();
            mock1.SetupGet(card => card.MonsterType).Returns(MonsterType.Human);
            Mock<ICard> humanMock = mock1.As<ICard>();
            humanMock.SetupGet(card => card.Element).Returns(Element.Fire);
            humanMock.SetupGet(card => card.Damage).Returns(100);
            humanMock.SetupGet(card => card.Specialties).Returns(new List<ISpecialty> { new DamageBoost() });
            ICard humanCard = humanMock.Object;

            Mock<IMonsterCard> mock2 = new Mock<IMonsterCard>();
            mock2.SetupGet(card => card.MonsterType).Returns(MonsterType.Troll);
            Mock<ICard> trollMock = mock2.As<ICard>();
            trollMock.SetupGet(card => card.Element).Returns(Element.Ice);
            trollMock.SetupGet(card => card.Damage).Returns(10);
            trollMock.SetupGet(card => card.Specialties).Returns(new List<ISpecialty> { new DamageBoost() });
            ICard trollCard = trollMock.Object;

            Mock<IMonsterCard> mock3 = new Mock<IMonsterCard>();
            mock3.SetupGet(card => card.MonsterType).Returns(MonsterType.Knight);
            Mock<ICard> knightMock = mock3.As<ICard>();
            knightMock.SetupGet(card => card.Element).Returns(Element.Wood);
            knightMock.SetupGet(card => card.Damage).Returns(40);
            knightMock.SetupGet(card => card.Specialties).Returns(new List<ISpecialty> { new DamageBoost() });
            ICard knightCard = knightMock.Object;

            List<ICard> list1 = new List<ICard> { humanCard };
            List<ICard> list2 = new List<ICard> { trollCard, knightCard };

            IPlayer p1 = new Player("Player1", list1, true);
            IPlayer p2 = new Player("Player2", list2, true);

            Battle battle = new Battle(p1, p2);
            battle.StartGame();
            BattleResult result = battle.GetResult();
            Assert.IsFalse(result.Draw);
            Assert.AreEqual(p1.Username, result.Winner);
            Assert.AreEqual(p2.Username, result.Loser);
            Assert.Pass("Battle Player 1 Wins the Game");
        }

        [Test(Description = "Battle result is draw because there are 100 cards per player.")]
        public void BattleUntilHundredRoundsAndDraw()
        {
            ICard fireHuman = new MonsterCard("", 10, MonsterType.Human, Element.Fire, new ISpecialty[] { new DamageBoost() });
            ICard iceSpell = new SpellCard("", 10, Element.Ice, new ISpecialty[] { new DamageBoost() });

            // How to pass 100 same cards to list
            // See: https://stackoverflow.com/a/17169142
            List<ICard> list1 = Enumerable.Repeat(fireHuman, 100).ToList();
            List<ICard> list2 = Enumerable.Repeat(iceSpell, 100).ToList();

            IPlayer p1 = new Player("Player1", list1, true);
            IPlayer p2 = new Player("Player2", list2, true);

            Battle battle = new Battle(p1, p2);
            battle.StartGame();
            BattleResult result = battle.GetResult();
            Assert.IsTrue(result.Draw);
            Assert.Pass("Battle Draw Game");
        }
    }
}