using MTCG.Data.Cards.Monster;
using MTCG.Data.Cards.Types;
using MTCG.Logging;

namespace MTCG.Data.Cards.Specialties
{
    public class AfraidOfDragons : ISpecialty
    {
        public void ApplyEffect(ICard card, ICard other, ref float myDamage, ref float otherDamage, in IBattleLog battleLog)
        {
            // miss the attack
            if (other is not MonsterCard { MonsterType: MonsterType.Dragon }) return;
            myDamage = 0;
            battleLog.AddEffectInfo($"{card.Name} is afraid of {other.Name}");
        }
    }
}