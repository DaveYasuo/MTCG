using System;
using System.Collections.Generic;
using System.Linq;
using DebugAndTrace;
using MTCG.Data.Cards.Monster;
using MTCG.Data.Cards.Specialties;
using MTCG.Data.Cards.Spell;
using MTCG.Data.Cards.Types;
using MTCG.Models;
using MTCG.Utilities;

namespace MTCG.Data.Cards
{
    public static class CardFactory
    {
        public static ICard BuildCard(Card card)
        {
            return card.GetCardType() switch
            {
                CardType.Monster => MonsterCard(card),
                CardType.Spell => SpellCard(card),
                _ => null
            };
        }

        public static CardType GetCardType(this Card card)
        {
            return card.Name.EndsWith("Spell") ? CardType.Spell : CardType.Monster;
        }

        private static MonsterType GetMonsterType(this Card card)
        {
            return card.Name.Parse<MonsterType>();
        }

        private static Element GetCardElement(this Card card)
        {
            try
            {
                return card.Name.Parse<Element>();
            }
            catch (InvalidOperationException)
            {
                var element = card.GenerateElement();
                card.Name = element + card.Name;
                return element;
            }
        }

        private static ICard SpellCard(Card card)
        {
            var specialties = new List<ISpecialty>();
            var element = card.GetCardElement();
            switch (element)
            {
                case Element.Water:
                    break;
                case Element.Regular:
                    specialties.Add(new DamageBoost());
                    break;
                case Element.Fire:
                    break;
                case Element.Ice:
                    break;
                case Element.Wind:
                    break;
                case Element.Earth:
                    break;
                case Element.Wood:
                    break;
                default:
                    TraceLogger.Instance.WriteLine("new SpellCard does not have any specialties.");
                    break;
            }

            return new SpellCard(card.Name, card.Damage, element, specialties);
        }

        private static ICard MonsterCard(Card card)
        {
            var monsterType = card.GetMonsterType();
            var element = card.GetCardElement();
            // Add special effects
            var specialties = new List<ISpecialty>();
            switch (monsterType)
            {
                case MonsterType.Goblin:
                    specialties.Add(new AfraidOfDragons());
                    break;
                case MonsterType.Troll:
                    break;
                case MonsterType.Human:
                    break;
                case MonsterType.Dragon:
                    specialties.Add(new EvadedByFireElf());
                    break;
                case MonsterType.Wizard:
                    specialties.Add(new ControlOrk());
                    break;
                case MonsterType.Ork:
                    break;
                case MonsterType.Knight:
                    specialties.Add(new DrownByWaterSpell());
                    break;
                case MonsterType.Kraken:
                    specialties.Add(new ImmunAgainstSpells());
                    break;
                case MonsterType.Elf:
                    break;
                default:
                    TraceLogger.Instance.WriteLine("new MonsterType does not have any specialties.");
                    break;
            }

            return new MonsterCard(card.Name, card.Damage, monsterType, element, specialties);
        }

        /// <summary>
        ///     Generic method for getting an enum from a string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>The single enum that was found, if no matches or when more than one enum was found, it throws an Exception</returns>
        private static T Parse<T>(this string value) where T : struct
        {
            // Check if string is in the enum list
            // See: https://stackoverflow.com/a/49792748
            // And: https://stackoverflow.com/a/41839209
            Enum.TryParse(Enum.GetNames(typeof(T)).Single(value.Contains), false, out T result);
            return result;
        }
    }
}