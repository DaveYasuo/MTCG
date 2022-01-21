using System;
using MTCG.Data.Cards;
using MTCG.Data.Cards.Specialties;
using MTCG.Data.Cards.Types;
using MTCG.Models;

namespace MTCG.Data.Utils
{
    public static class Utility
    {
        private static readonly Random Rnd = new();

        /// <summary>
        /// Calculates damage for both cards.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="other"></param>
        /// <returns>A Tuple with the float damage of card1 and card2</returns>
        public static (float, float) CalculateDamage(ICard card, ICard other)
        {
            // default damages
            float myDamage = card.Damage, otherDamage = other.Damage;
            if (card is SpellCard || other is SpellCard) card.ApplySpellEffect(other, ref myDamage, ref otherDamage);
            // player1
            foreach (ISpecialty specialty in card.Specialties) specialty.ApplyEffect(card, other, ref myDamage, ref otherDamage);
            // player2
            foreach (ISpecialty specialty in other.Specialties) specialty.ApplyEffect(other, card, ref otherDamage, ref myDamage);
            return (myDamage, otherDamage);
        }

        private static void ApplySpellEffect(this ICard card, ICard other, ref float myDamage, ref float otherDamage)
        {
            if (card.Element == other.Element) return;
            switch (card.Element)
            {
                case Element.Water when other.Element == Element.Fire:
                    MultiplyAndDivideBy(ref myDamage, ref otherDamage, 2);
                    return;
                case Element.Water when other.Element == Element.Regular:
                    MultiplyAndDivideBy(ref otherDamage, ref myDamage, 2);
                    return;
                case Element.Regular when other.Element == Element.Water:
                    MultiplyAndDivideBy(ref myDamage, ref otherDamage, 2);
                    return;
                case Element.Regular when other.Element == Element.Fire:
                    MultiplyAndDivideBy(ref otherDamage, ref myDamage, 2);
                    return;
                case Element.Fire when other.Element == Element.Water:
                    MultiplyAndDivideBy(ref otherDamage, ref myDamage, 2);
                    return;
                case Element.Fire when other.Element == Element.Regular:
                    MultiplyAndDivideBy(ref myDamage, ref otherDamage, 2);
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