using MTCG.Data.Cards.Spell;
using MTCG.Data.Cards.Types;
using MTCG.Logging;

namespace MTCG.Data.Cards.Specialties
{
    public class DrownByWaterSpell : ISpecialty
    {
        public void ApplyEffect(ICard card, ICard other, ref float myDamage, ref float otherDamage,
            in IBattleLog battleLog)
        {
            if (other is not SpellCard { Element: Element.Water }) return;
            myDamage = 0;
            battleLog.AddEffectInfo($"{card.Name} can be drown by {other.Name}");
        }
    }
}