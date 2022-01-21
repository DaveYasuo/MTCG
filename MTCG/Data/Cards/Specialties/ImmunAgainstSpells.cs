using MTCG.Logging;

namespace MTCG.Data.Cards.Specialties
{
    public class ImmunAgainstSpells : ISpecialty
    {
        public void ApplyEffect(ICard card, ICard other, ref float myDamage, ref float otherDamage, in IBattleLog battleLog)
        {
            if (other is not SpellCard) return;
            otherDamage = 0;
            battleLog.AddEffectInfo($"{card.Name} is immun against {other.Name}");
        }
    }
}