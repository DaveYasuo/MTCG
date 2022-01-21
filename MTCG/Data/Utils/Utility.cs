using System;
using MTCG.Data.Cards;
using MTCG.Data.Cards.Specialties;
using MTCG.Data.Cards.Types;
using MTCG.Logging;
using MTCG.Models;

namespace MTCG.Data.Utils
{
    public static class Utility
    {
        private static readonly Random Rnd = new();

        /// <summary>
        /// Calculates damage for both cards and add logs.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="other"></param>
        /// <param name="battleLog"></param>
        /// <returns>A Tuple with the float damage of card1 and card2</returns>
        public static (float damage1, float damage2) CalculateDamage(ICard card, ICard other, in IBattleLog battleLog)
        {
            // default damages
            float myDamage = card.Damage, otherDamage = other.Damage;
            battleLog.AddBaseDamage(myDamage, otherDamage);
            if (card is SpellCard || other is SpellCard) card.ApplySpellEffect(other, ref myDamage, ref otherDamage, in battleLog);
            // player1
            foreach (ISpecialty specialty in card.Specialties) specialty.ApplyEffect(card, other, ref myDamage, ref otherDamage, in battleLog);
            // player2
            foreach (ISpecialty specialty in other.Specialties) specialty.ApplyEffect(other, card, ref otherDamage, ref myDamage, in battleLog);
            battleLog.AddEffectiveDamage(myDamage, otherDamage);
            return (myDamage, otherDamage);
        }

        /// <summary>
        /// Calculates elemental damages and log it to the battle log.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="other"></param>
        /// <param name="myDamage"></param>
        /// <param name="otherDamage"></param>
        /// <param name="battleLog"></param>
        private static void ApplySpellEffect(this ICard card, ICard other, ref float myDamage, ref float otherDamage, in IBattleLog battleLog)
        {
            if (card.Element == other.Element) return;
            switch (card.Element)
            {
                case Element.Water when other.Element == Element.Fire:
                case Element.Fire when other.Element == Element.Regular:
                case Element.Regular when other.Element == Element.Water:
                    battleLog.AddElementReaction(card.Element.ToString(), other.Element.ToString());
                    MultiplyAndDivideBy(ref myDamage, ref otherDamage, 2);
                    return;
                case Element.Water when other.Element == Element.Regular:
                case Element.Regular when other.Element == Element.Fire:
                case Element.Fire when other.Element == Element.Water:
                    battleLog.AddElementReaction(other.Element.ToString(), card.Element.ToString());
                    MultiplyAndDivideBy(ref otherDamage, ref myDamage, 2);
                    return;
                default: return;
            }
        }

        /// <summary>
        /// Multiply first argument by factor and divide second argument by factor
        /// </summary>
        /// <param name="multiply"></param>
        /// <param name="divide"></param>
        /// <param name="factor"></param>
        private static void MultiplyAndDivideBy(ref float multiply, ref float divide, float factor)
        {
            multiply *= factor;
            divide /= factor;
        }

        /// <summary>
        /// Get a random Element of the Element Enum.
        /// </summary>
        /// <param name="card"></param>
        /// <returns>A random Element which is part of the Enum Element.</returns>
        public static Element GenerateElement(this Card card)
        {
            // Get Random Enum
            // See: https://stackoverflow.com/a/3132151
            Array values = Enum.GetValues(typeof(Element));
            // Null suppression
            // See: https://stackoverflow.com/a/54724546
            return (Element)values.GetValue(Rnd.Next(values.Length))!;
        }
    }
}