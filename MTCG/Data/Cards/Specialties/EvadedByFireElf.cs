using MTCG.Data.Cards.Monster;
using MTCG.Data.Cards.Types;
using MTCG.Logging;

namespace MTCG.Data.Cards.Specialties
{
    public class EvadedByFireElf : ISpecialty
    {
        public void ApplyEffect(ICard card, ICard other, ref float myDamage, ref float otherDamage,
            in IBattleLog battleLog)
        {
            // missed 
            if (other is not MonsterCard { MonsterType: MonsterType.Elf, Element: Element.Fire }) return;
            myDamage = 0;
            battleLog.AddEffectInfo($"{card.Name} can be evaded by {other.Name}");
        }
    }
}