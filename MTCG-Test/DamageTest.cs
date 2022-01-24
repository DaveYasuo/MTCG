using System;
using System.Collections.Generic;
using Moq;
using MTCG.Data.Cards;
using MTCG.Data.Cards.Monster;
using MTCG.Data.Cards.Specialties;
using MTCG.Data.Cards.Spell;
using MTCG.Data.Cards.Types;
using MTCG.Logging;
using MTCG.Utilities;
using NUnit.Framework;

namespace MTCG_Test
{
    [TestFixture]
    public class DamageTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            _battleLog = new Mock<IBattleLog>().Object;
        }

        private IBattleLog _battleLog;

        [TestCase(0F, 0F)]
        [TestCase(10, 10)]
        [TestCase(1234.1234F, 0F)]
        [TestCase(50, 100.32F)]
        [TestCase(100, 100)]
        public void WaterDragonEvadedByFireElvesVsFireElf(float rawDamage1, float rawDamage2)
        {
            ICard player1Card = new MonsterCard("WaterDragon", rawDamage1, MonsterType.Dragon, Element.Water,
                new List<ISpecialty> { new EvadedByFireElf() });
            ICard player2Card = new MonsterCard("FireElf", rawDamage2, MonsterType.Elf, Element.Fire,
                new List<ISpecialty>());

            (var damage1, var damage2) = DamageUtility.CalculateDamage(player1Card, player2Card, _battleLog);
            Assert.Zero(damage1, "Dragon cannot hit FireElf, so zero damage");
            Assert.AreEqual(rawDamage2, damage2,
                $"FireElf can deal {rawDamage2} damage points, since there are no more specialties or effectiveness");
            Assert.LessOrEqual(damage1, damage2);
            Assert.Pass("WaterDragon VS FireElf passed the test.");
        }

        [TestCase(0F, 0F)]
        [TestCase(10, 10)]
        [TestCase(1234.1234F, 0F)]
        [TestCase(12, 32)]
        [TestCase(50, 10)]
        public void FireKrakenImmunAgainstWaterSpellVsWaterSpell(float rawDamage1, float rawDamage2)
        {
            ICard player1Card = new MonsterCard("FireKraken", rawDamage1, MonsterType.Kraken, Element.Fire,
                new List<ISpecialty> { new ImmunAgainstSpells() });
            ICard player2Card = new SpellCard("WaterSpell", rawDamage2, Element.Water, new List<ISpecialty>());

            (var damage1, var damage2) = DamageUtility.CalculateDamage(player1Card, player2Card, _battleLog);
            Assert.AreEqual(rawDamage1 / 2, damage1, "Fire is not effective against Water, so damage is halved");
            Assert.Zero(damage2, "Kraken are immun against Spells, so Spells deal no damage");
            Assert.GreaterOrEqual(damage1, damage2);
            Assert.Pass("FireKraken VS WaterSpell passed the test.");
        }

        [TestCase(30F, 21.12F)]
        [TestCase(10, 10)]
        [TestCase(1234.1234F, 0F)]
        [TestCase(12, 32)]
        [TestCase(50, 10)]
        public void DragonAfraidOfDragonsVsDragonAfraidOfDragons(float rawDamage1, float rawDamage2)
        {
            ICard player1Card = new MonsterCard("FireDragon", rawDamage1, MonsterType.Dragon, Element.Fire,
                new List<ISpecialty> { new AfraidOfDragons() });
            ICard player2Card = new MonsterCard("WaterDragon", rawDamage2, MonsterType.Dragon, Element.Water,
                new List<ISpecialty> { new AfraidOfDragons() });

            (var damage1, var damage2) = DamageUtility.CalculateDamage(player1Card, player2Card, _battleLog);
            Assert.Zero(damage1, "Since dragon is afraid of dragons, it deals no damage");
            Assert.Zero(damage2, "Since dragon is afraid of dragons, it deals no damage");
            Assert.AreEqual(damage1, damage2);
            Assert.Pass("Dragon VS Dragon with no damage passed the test.");
        }

        [TestCase(30F, 21.12F)]
        [TestCase(10, 10)]
        [TestCase(1234.1234F, 0F)]
        [TestCase(12, 32)]
        [TestCase(50, 10)]
        public void WoodWizardControlOrkVsIceOrk(float rawDamage1, float rawDamage2)
        {
            ICard player1Card = new MonsterCard("WoodWizard", rawDamage1, MonsterType.Wizard, Element.Wood,
                new List<ISpecialty> { new ControlOrk() });
            ICard player2Card =
                new MonsterCard("IceOrk", rawDamage2, MonsterType.Ork, Element.Ice, new List<ISpecialty>());

            (var damage1, var damage2) = DamageUtility.CalculateDamage(player1Card, player2Card, _battleLog);
            Assert.AreEqual(rawDamage1, damage1, "No effectiveness which modifies the damage");
            Assert.Zero(damage2, "Since Ork are controlled by wizards, no damage are dealt");
            Assert.GreaterOrEqual(damage1, damage2);
            Assert.Pass("Wizard controlling Ork passed the test.");
        }

        [TestCase(30F, 21.12F)]
        [TestCase(10, 10)]
        [TestCase(1234.1234F, 0F)]
        [TestCase(12, 32)]
        [TestCase(50, 10)]
        public void BonusRoundDamageTest(float rawDamage1, float rawDamage2)
        {
            var damage1 = rawDamage1;
            var damage2 = rawDamage2;
            DamageUtility.BonusRound(ref damage1, ref damage2, _battleLog);
            Assert.GreaterOrEqual(damage1, rawDamage1);
            Assert.GreaterOrEqual(damage2, rawDamage2);
            Assert.Pass("Both damages can be greater than before.");
        }

        [TestCase(Element.Regular, Element.Water)]
        [TestCase(Element.Water, Element.Earth)]
        [TestCase(Element.Wind, Element.Earth)]
        [TestCase(Element.Ice, Element.Wood)]
        [TestCase(Element.Wind, Element.Fire)]
        [TestCase(Element.Wood, Element.Earth)]
        [TestCase(Element.Wood, Element.Wind)]
        [TestCase(Element.Earth, Element.Fire)]
        [TestCase(Element.Earth, Element.Ice)]
        public void Spell10VsSpell10WhereFirstIsEffectiveAgainstSecond(Element element1, Element element2)
        {
            ICard player1Card = new SpellCard("SomeSpell", 10, element1, new List<ISpecialty>());
            ICard player2Card = new SpellCard("SomeSpell", 10, element2, new List<ISpecialty>());

            (var damage1, var damage2) = DamageUtility.CalculateDamage(player1Card, player2Card, _battleLog);
            Assert.AreEqual(20, damage1, $"{element1} is effective against {element2} so damage is doubled");
            Assert.AreEqual(5, damage2, $"{element2} is not effective against {element1} so damage is halved");
            Assert.GreaterOrEqual(damage1, damage2);
            Assert.Pass("Spell VS Spell passed the test.");
        }

        [TestCase(Element.Regular, Element.Fire)]
        [TestCase(Element.Water, Element.Ice)]
        [TestCase(Element.Wind, Element.Ice)]
        [TestCase(Element.Ice, Element.Earth)]
        [TestCase(Element.Wind, Element.Wood)]
        [TestCase(Element.Wood, Element.Ice)]
        [TestCase(Element.Wood, Element.Fire)]
        [TestCase(Element.Earth, Element.Wood)]
        [TestCase(Element.Earth, Element.Wind)]
        public void Spell10VsSpell10WhereSecondIsEffectiveAgainstFirst(Element element1, Element element2)
        {
            ICard player1Card = new SpellCard("SomeSpell", 10, element1, new List<ISpecialty>());
            ICard player2Card = new SpellCard("SomeSpell", 10, element2, new List<ISpecialty>());

            (var damage1, var damage2) = DamageUtility.CalculateDamage(player1Card, player2Card, _battleLog);
            Assert.AreEqual(5, damage1, $"{element1} is not effective against {element2} so damage is halved");
            Assert.AreEqual(20, damage2, $"{element2} is effective against {element1} so damage is doubled");
            Assert.LessOrEqual(damage1, damage2);
            Assert.Pass("Spell VS Spell passed the test.");
        }

        [TestCase(0F, 0F)]
        [TestCase(10, 10)]
        [TestCase(1234.1234F, 0F)]
        [TestCase(12, 32)]
        [TestCase(50, 10)]
        public void IceKnightGetsDrownByWaterSpellVsWaterSpellWithDamageBoost(float rawDamage1, float rawDamage2)
        {
            ICard player1Card = new MonsterCard("IceKnight", rawDamage1, MonsterType.Knight, Element.Ice,
                new List<ISpecialty> { new DrownByWaterSpell() });
            ICard player2Card = new SpellCard("WaterSpell", rawDamage2, Element.Water,
                new List<ISpecialty> { new DamageBoost() });

            (var damage1, var damage2) = DamageUtility.CalculateDamage(player1Card, player2Card, _battleLog);
            Assert.Zero(damage1, "Knight gets drowned by WaterSpell, so zero damage");
            Assert.IsTrue(Math.Abs(damage2 - rawDamage2) < 0.01 || Math.Abs(damage2 - rawDamage2 / 2) < 0.01,
                "WaterSpell is not effective against Ice, so damage is halved but it also has a fifty percent chance to deal double damage");
            Assert.Pass("FireKraken VS WaterSpell passed the test.");
        }
    }
}