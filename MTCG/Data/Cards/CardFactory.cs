using System;
using System.Collections.Generic;
using DebugAndTrace;
using MTCG.Data.Cards.Specialties;
using MTCG.Data.Cards.Types;
using MTCG.Models;

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

        /// <summary>
        /// Generic method for getting for example an enum from a string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns>True if conversion is success, otherwise false</returns>
        private static (bool, T result) IsParsable<T>(this string value) where T : struct
        {
            // Check if string is in the enum list
            // See: https://stackoverflow.com/a/49792748
            return (Enum.TryParse<T>(value, true, out T result), result);
        }

        private static MonsterType GetMonsterType(this Card card)
        {
            (bool result, MonsterType monsterType) = card.Name.IsParsable<MonsterType>();
            // if no matches, we can use human instead :P
            return result ? monsterType : MonsterType.Human;
        }

        private static CardType GetCardType(this Card card)
        {
            return card.Name.Contains("Spell") ? CardType.Spell : CardType.Monster;
        }

        private static Element GetCardElement(this Card card)
        {
            (bool result, Element element) = card.Name.IsParsable<Element>();
            // if no matches, we use none element 
            return result ? element : Element.None;
        }

        private static ICard SpellCard(Card card)
        {
            // No specialties by now todo
            List<ISpecialty> specialties = new List<ISpecialty>();
            Element element = card.GetCardElement();
            switch (element)
            {
                case Element.Water:
                    break;
                case Element.Regular:
                    break;
                case Element.Fire:
                    break;
                case Element.None:
                    break;
                default:
                    TraceLogger.Instance.WriteLine("new SpellCard does not have any specialties.");
                    break;
            }
            return new SpellCard(card, element, specialties);
        }

        private static ICard MonsterCard(Card card)
        {
            MonsterType monsterType = card.GetMonsterType();
            Element element = card.GetCardElement();
            // Add special effects
            List<ISpecialty> specialties = new List<ISpecialty>();
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
            return new MonsterCard(card, monsterType, element, specialties);
        }
    }
}