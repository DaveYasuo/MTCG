using MTCG.Logging;
using MTCG.Utilities;

namespace MTCG.Data.Cards.Specialties
{
    public class DamageBoost : ISpecialty
    {
        public void ApplyEffect(ICard card, ICard other, ref float myDamage, ref float otherDamage,
            in IBattleLog battleLog)
        {
            // this card has a fifty percent chance to deal double damage
            if (100.GetRandomIntegerBetween() > 50) return;
            myDamage *= 2;
            battleLog.AddEffectInfo(
                $"{card.Name} finds a weakness of {other.Name} and deals additional double damage.");
        }
    }
}