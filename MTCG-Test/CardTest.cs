using System;
using System.Linq;
using MTCG.Data.Cards;
using MTCG.Data.Cards.Monster;
using MTCG.Data.Cards.Specialties;
using MTCG.Data.Cards.Spell;
using MTCG.Data.Cards.Types;
using MTCG.Models;
using NUnit.Framework;

namespace MTCG_Test
{
    [TestFixture]
    public class CardTests
    {
 

        [Test]
        public void CreateDragonWithRandomElement()
        {
            var rawCard = new Card(Guid.Empty, "Dragon", 10);
            Assert.AreEqual(CardType.Monster, rawCard.GetCardType());
            var card = CardFactory.BuildCard(rawCard);
            Assert.AreEqual(rawCard.Name, card.Name);
            Assert.IsInstanceOf<MonsterCard>(card);
            var monsterCard = card as MonsterCard;
            Assert.AreEqual(MonsterType.Dragon, monsterCard!.MonsterType);
            Assert.That(monsterCard.Specialties.OfType<EvadedByFireElf>().Any);
            Assert.Pass("Dragon card with random Element type.");
        }

        [Test]
        public void CreateNonExistsMonsterCardThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var rawCard = new Card(Guid.Empty, "NoCard", 10);
                Assert.AreEqual(CardType.Monster, rawCard.GetCardType());
                CardFactory.BuildCard(rawCard);
            });
        }

        [TestCase("RegularSpell", Element.Regular)]
        [TestCase("FireSpell", Element.Fire)]
        [TestCase("WaterSpell", Element.Water)]
        [TestCase("IceSpell", Element.Ice)]
        [TestCase("WindSpell", Element.Wind)]
        [TestCase("EarthSpell", Element.Earth)]
        [TestCase("WoodSpell", Element.Wood)]
        public void CreateAllSpellCards(string spellCardName, Element expectedElement)
        {
            var rawCard = new Card(Guid.Empty, spellCardName, 10);
            Assert.AreEqual(CardType.Spell, rawCard.GetCardType());

            var card = CardFactory.BuildCard(rawCard);
            Assert.AreEqual(rawCard.Name, card.Name);
            Assert.AreEqual(expectedElement, card.Element);
            Assert.IsInstanceOf<SpellCard>(card);
            Assert.Pass(spellCardName + " passed build test.");
        }

        [TestCase("FireDragon", Element.Fire)]
        [TestCase("WaterDragon", Element.Water)]
        [TestCase("IceDragon", Element.Ice)]
        [TestCase("WindDragon", Element.Wind)]
        [TestCase("WoodDragon", Element.Wood)]
        [TestCase("EarthDragon", Element.Earth)]
        [TestCase("RegularDragon", Element.Regular)]
        public void TestAllElementsOfDragonCardWithEvadedByFireElfSpecialty(string monsterCardName, Element expectedElement)
        {
            CreateSpecificMonsterCard<EvadedByFireElf>(monsterCardName, expectedElement, MonsterType.Dragon);
        }

        [TestCase("FireKnight", Element.Fire)]
        [TestCase("WaterKnight", Element.Water)]
        [TestCase("IceKnight", Element.Ice)]
        [TestCase("WindKnight", Element.Wind)]
        [TestCase("WoodKnight", Element.Wood)]
        [TestCase("EarthKnight", Element.Earth)]
        [TestCase("RegularKnight", Element.Regular)]
        public void TestAllElementsOfKnightCardWithDrownByWaterSpellSpecialty(string monsterCardName, Element expectedElement)
        {
            CreateSpecificMonsterCard<DrownByWaterSpell>(monsterCardName, expectedElement, MonsterType.Knight);
        }

        [TestCase("FireWizard", Element.Fire)]
        [TestCase("WaterWizard", Element.Water)]
        [TestCase("IceWizard", Element.Ice)]
        [TestCase("WindWizard", Element.Wind)]
        [TestCase("WoodWizard", Element.Wood)]
        [TestCase("EarthWizard", Element.Earth)]
        [TestCase("RegularWizard", Element.Regular)]
        public void TestAllElementsOfWizardCardWithControlOrkSpecialty(string monsterCardName, Element expectedElement)
        {
            CreateSpecificMonsterCard<ControlOrk>(monsterCardName, expectedElement, MonsterType.Wizard);
        }

        [TestCase("FireGoblin", Element.Fire)]
        [TestCase("WaterGoblin", Element.Water)]
        [TestCase("IceGoblin", Element.Ice)]
        [TestCase("WindGoblin", Element.Wind)]
        [TestCase("WoodGoblin", Element.Wood)]
        [TestCase("EarthGoblin", Element.Earth)]
        [TestCase("RegularGoblin", Element.Regular)]
        public void TestAllElementsOfGoblinCardWithAfraidOfDragonsSpecialty(string monsterCardName, Element expectedElement)
        {
            CreateSpecificMonsterCard<AfraidOfDragons>(monsterCardName, expectedElement, MonsterType.Goblin);
        }

        [TestCase("FireKraken", Element.Fire)]
        [TestCase("WaterKraken", Element.Water)]
        [TestCase("IceKraken", Element.Ice)]
        [TestCase("WindKraken", Element.Wind)]
        [TestCase("WoodKraken", Element.Wood)]
        [TestCase("EarthKraken", Element.Earth)]
        [TestCase("RegularKraken", Element.Regular)]
        public void TestAllElementsOfKrakenCardWithImmunAgainstSpellsSpecialty(string monsterCardName, Element expectedElement)
        {
            CreateSpecificMonsterCard<ImmunAgainstSpells>(monsterCardName, expectedElement, MonsterType.Kraken);
        }

        [TestCase("FireHuman", Element.Fire)]
        [TestCase("WaterHuman", Element.Water)]
        [TestCase("IceHuman", Element.Ice)]
        [TestCase("WindHuman", Element.Wind)]
        [TestCase("WoodHuman", Element.Wood)]
        [TestCase("EarthHuman", Element.Earth)]
        [TestCase("RegularHuman", Element.Regular)]
        public void TestAllElementsOfHumanCardWithNoSpecialty(string monsterCardName, Element expectedElement)
        {
            CreateSpecificMonsterCard(monsterCardName, expectedElement, MonsterType.Human);
        }

        [TestCase("FireTroll", Element.Fire)]
        [TestCase("WaterTroll", Element.Water)]
        [TestCase("IceTroll", Element.Ice)]
        [TestCase("WindTroll", Element.Wind)]
        [TestCase("WoodTroll", Element.Wood)]
        [TestCase("EarthTroll", Element.Earth)]
        [TestCase("RegularTroll", Element.Regular)]
        public void TestAllElementsOfTrollCardWithNoSpecialty(string monsterCardName, Element expectedElement)
        {
            CreateSpecificMonsterCard(monsterCardName, expectedElement, MonsterType.Troll);
        }

        [TestCase("FireOrk", Element.Fire)]
        [TestCase("WaterOrk", Element.Water)]
        [TestCase("IceOrk", Element.Ice)]
        [TestCase("WindOrk", Element.Wind)]
        [TestCase("WoodOrk", Element.Wood)]
        [TestCase("EarthOrk", Element.Earth)]
        [TestCase("RegularOrk", Element.Regular)]
        public void TestAllElementsOfOrkCardWithNoSpecialty(string monsterCardName, Element expectedElement)
        {
            CreateSpecificMonsterCard(monsterCardName, expectedElement, MonsterType.Ork);
        }

        [TestCase("FireElf", Element.Fire)]
        [TestCase("WaterElf", Element.Water)]
        [TestCase("IceElf", Element.Ice)]
        [TestCase("WindElf", Element.Wind)]
        [TestCase("WoodElf", Element.Wood)]
        [TestCase("EarthElf", Element.Earth)]
        [TestCase("RegularElf", Element.Regular)]
        public void TestAllElementsOfElfCardWithNoSpecialty(string monsterCardName, Element expectedElement)
        {
            CreateSpecificMonsterCard(monsterCardName, expectedElement, MonsterType.Elf);
        }


        private static void CreateSpecificMonsterCard<T>(string monsterCardName, Element expectedElement,
            MonsterType expectedMonsterType)
        {
            var rawCard = new Card(Guid.Empty, monsterCardName, 10);
            Assert.AreEqual(CardType.Monster, rawCard.GetCardType());

            var card = CardFactory.BuildCard(rawCard);
            Assert.AreEqual(rawCard.Name, card.Name);
            Assert.AreEqual(expectedElement, card.Element);
            Assert.IsInstanceOf<MonsterCard>(card);
            var monsterCard = card as MonsterCard;
            Assert.AreEqual(expectedMonsterType, monsterCard!.MonsterType);
            Assert.That(monsterCard.Specialties.OfType<T>().Any);
            Assert.Pass(monsterCardName + " passed build test.");
        }
 
        private static void CreateSpecificMonsterCard(string monsterCardName, Element expectedElement,
            MonsterType expectedMonsterType)
        {
            var rawCard = new Card(Guid.Empty, monsterCardName, 10);
            Assert.AreEqual(CardType.Monster, rawCard.GetCardType());

            var card = CardFactory.BuildCard(rawCard);
            Assert.AreEqual(rawCard.Name, card.Name);
            Assert.AreEqual(expectedElement, card.Element);
            Assert.IsInstanceOf<MonsterCard>(card);
            var monsterCard = card as MonsterCard;
            Assert.AreEqual(expectedMonsterType, monsterCard!.MonsterType);
            Assert.Pass(monsterCardName + " passed build test.");
        }
    }
}