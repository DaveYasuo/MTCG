using MTCG.Data.Cards.Types;

namespace MTCG.Data.Cards.Specialties
{
    public class DrownByWaterSpell : ISpecialty
    {
        public void ApplyEffect(ICard card, ICard other, ref float myDamage, ref float otherDamage)
        {
            if (other is SpellCard { Element: Element.Water }) myDamage = 0;
        }
    }
}