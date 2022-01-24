using System.Collections.Generic;
using System.Linq;
using Moq;
using MTCG.BattleLogic;
using MTCG.Data.Cards;
using MTCG.Data.Cards.Monster;
using MTCG.Data.Cards.Specialties;
using MTCG.Data.Cards.Spell;
using MTCG.Data.Cards.Types;
using MTCG.Data.Users;
using NUnit.Framework;

namespace MTCG_Test
{
    [TestFixture]
    public class BattleTest
    {
        [Test]
        public void BattleOneRoundPlayer1Wins()
        {
            var mock1 = new Mock<IMonsterCard>();
            mock1.Setup(card => card.MonsterType).Returns(MonsterType.Human);
            var humanMock = mock1.As<ICard>();
            humanMock.Setup(card => card.Damage).Returns(100);
            var human = humanMock.Object;

            var mock2 = new Mock<IMonsterCard>();
            mock2.Setup(card => card.MonsterType).Returns(MonsterType.Troll);
            var trollMock = mock2.As<ICard>();
            trollMock.Setup(card => card.Damage).Returns(50);
            var troll = trollMock.Object;


            var player1Mock = new Mock<IPlayer>();
            player1Mock.SetupGet(player => player.Username).Returns("Player1");
            player1Mock.SetupGet(player => player.Log).Returns(new List<object>());
            player1Mock.Setup(player => player.Cards).Returns(new List<ICard>());
            player1Mock.Setup(player => player.GetRandomCard()).Returns(human);
            var player1 = player1Mock.Object;

            var player2Mock = new Mock<IPlayer>();
            player2Mock.SetupGet(player => player.Username).Returns("Player2");
            player2Mock.SetupGet(player => player.Log).Returns(new List<object>());
            player2Mock.Setup(player => player.Cards).Returns(new List<ICard>());
            player2Mock.Setup(player => player.GetRandomCard()).Returns(troll);
            var player2 = player2Mock.Object;

            var battle = new Battle(player1, player2);
            battle.StartGame();
            var result = battle.GetResult();
            Assert.IsFalse(result.Draw);
            Assert.AreEqual(player1.Username, result.Winner);
            Assert.AreEqual(player2.Username, result.Loser);
            Assert.Pass("Battle Player 1 wins");
        }

        [Test]
        public void BattleOneRoundPlayer2Wins()
        {
            var mock1 = new Mock<IMonsterCard>();
            mock1.Setup(card => card.MonsterType).Returns(MonsterType.Human);
            var humanMock = mock1.As<ICard>();
            humanMock.Setup(card => card.Damage).Returns(50);
            var human = humanMock.Object;

            var mock2 = new Mock<IMonsterCard>();
            mock2.Setup(card => card.MonsterType).Returns(MonsterType.Troll);
            var trollMock = mock2.As<ICard>();
            trollMock.Setup(card => card.Damage).Returns(100);
            var troll = trollMock.Object;


            var player1Mock = new Mock<IPlayer>();
            player1Mock.SetupGet(player => player.Username).Returns("Player1");
            player1Mock.SetupGet(player => player.Log).Returns(new List<object>());
            player1Mock.Setup(player => player.Cards).Returns(new List<ICard>());
            player1Mock.Setup(player => player.GetRandomCard()).Returns(human);
            var player1 = player1Mock.Object;

            var player2Mock = new Mock<IPlayer>();
            player2Mock.SetupGet(player => player.Username).Returns("Player2");
            player2Mock.SetupGet(player => player.Log).Returns(new List<object>());
            player2Mock.Setup(player => player.Cards).Returns(new List<ICard>());
            player2Mock.Setup(player => player.GetRandomCard()).Returns(troll);
            var player2 = player2Mock.Object;

            var battle = new Battle(player1, player2);
            battle.StartGame();
            var result = battle.GetResult();
            Assert.IsFalse(result.Draw);
            Assert.AreEqual(player1.Username, result.Loser);
            Assert.AreEqual(player2.Username, result.Winner);
            Assert.Pass("Battle Player 2 wins");
        }

        [Test(Description = "Battle result is draw since both player have the same card")]
        public void BattleOneRoundDrawGame()
        {
            var mock1 = new Mock<IMonsterCard>();
            mock1.Setup(card => card.MonsterType).Returns(MonsterType.Human);
            var humanMock = mock1.As<ICard>();
            humanMock.Setup(card => card.Damage).Returns(100);
            var human = humanMock.Object;

            var player1Mock = new Mock<IPlayer>();
            player1Mock.SetupGet(player => player.Username).Returns("Player1");
            player1Mock.SetupGet(player => player.Log).Returns(new List<object>());
            player1Mock.Setup(player => player.Cards).Returns(new List<ICard>());
            player1Mock.Setup(player => player.GetRandomCard()).Returns(human);
            var player1 = player1Mock.Object;

            var player2Mock = new Mock<IPlayer>();
            player2Mock.SetupGet(player => player.Username).Returns("Player2");
            player2Mock.SetupGet(player => player.Log).Returns(new List<object>());
            player2Mock.Setup(player => player.Cards).Returns(new List<ICard>());
            player2Mock.Setup(player => player.GetRandomCard()).Returns(human);
            var player2 = player2Mock.Object;

            var battle = new Battle(player1, player2);
            battle.StartGame();
            var result = battle.GetResult();
            Assert.IsTrue(result.Draw);
            Assert.Pass("Battle Draw Game");
        }

        [Test(Description =
            "Battle result can not draw even though there is a bonus round every ten rounds and fifty percent chance to deal double damage."
            + " Player2's WoodKnight's max damage is 80 and Player1's FireHuman's min damage is 100.")]
        public void BattleUntilPlayerOneWins()
        {
            var mock1 = new Mock<IMonsterCard>();
            mock1.SetupGet(card => card.MonsterType).Returns(MonsterType.Human);
            var humanMock = mock1.As<ICard>();
            humanMock.SetupGet(card => card.Element).Returns(Element.Fire);
            humanMock.SetupGet(card => card.Damage).Returns(100);
            humanMock.SetupGet(card => card.Specialties).Returns(new List<ISpecialty> { new DamageBoost() });
            var humanCard = humanMock.Object;

            var mock2 = new Mock<IMonsterCard>();
            mock2.SetupGet(card => card.MonsterType).Returns(MonsterType.Troll);
            var trollMock = mock2.As<ICard>();
            trollMock.SetupGet(card => card.Element).Returns(Element.Ice);
            trollMock.SetupGet(card => card.Damage).Returns(10);
            trollMock.SetupGet(card => card.Specialties).Returns(new List<ISpecialty> { new DamageBoost() });
            var trollCard = trollMock.Object;

            var mock3 = new Mock<IMonsterCard>();
            mock3.SetupGet(card => card.MonsterType).Returns(MonsterType.Knight);
            var knightMock = mock3.As<ICard>();
            knightMock.SetupGet(card => card.Element).Returns(Element.Wood);
            knightMock.SetupGet(card => card.Damage).Returns(40);
            knightMock.SetupGet(card => card.Specialties).Returns(new List<ISpecialty> { new DamageBoost() });
            var knightCard = knightMock.Object;

            var list1 = new List<ICard> { humanCard };
            var list2 = new List<ICard> { trollCard, knightCard };

            IPlayer p1 = new Player("Player1", list1, true);
            IPlayer p2 = new Player("Player2", list2, true);

            var battle = new Battle(p1, p2);
            battle.StartGame();
            var result = battle.GetResult();
            Assert.IsFalse(result.Draw);
            Assert.AreEqual(p1.Username, result.Winner);
            Assert.AreEqual(p2.Username, result.Loser);
            Assert.Pass("Battle Player 1 Wins the Game");
        }

        [Test(Description = "Battle result is draw because there are 100 cards per player.")]
        public void BattleUntilHundredRoundsAndDraw()
        {
            ICard fireHuman = new MonsterCard("", 10, MonsterType.Human, Element.Fire,
                new ISpecialty[] { new DamageBoost() });
            ICard iceSpell = new SpellCard("", 10, Element.Ice, new ISpecialty[] { new DamageBoost() });

            // How to pass 100 same cards to list
            // See: https://stackoverflow.com/a/17169142
            var list1 = Enumerable.Repeat(fireHuman, 100).ToList();
            var list2 = Enumerable.Repeat(iceSpell, 100).ToList();

            IPlayer p1 = new Player("Player1", list1, true);
            IPlayer p2 = new Player("Player2", list2, true);

            var battle = new Battle(p1, p2);
            battle.StartGame();
            var result = battle.GetResult();
            Assert.IsTrue(result.Draw);
            Assert.Pass("Battle Draw Game");
        }
    }
}