using MTCG.Data.Cards.Monster;
using MTCG.Data.Cards.Types;
using MTCG.Logging;

namespace MTCG.Data.Cards.Specialties
{
    public class ControlOrk : ISpecialty
    {
        public void ApplyEffect(ICard card, ICard other, ref float myDamage, ref float otherDamage,
            in IBattleLog battleLog)
        {
            // enemy missed
            if (other is not MonsterCard { MonsterType: MonsterType.Ork }) return;
            otherDamage = 0;
            battleLog.AddEffectInfo($"{card.Name} can control {other.Name}");
        }
    }
}