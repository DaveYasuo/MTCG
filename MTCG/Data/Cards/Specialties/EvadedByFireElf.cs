using MTCG.Data.Cards.Types;

namespace MTCG.Data.Cards.Specialties
{
    public class EvadedByFireElf : ISpecialty
    {
        public void ApplyEffect(ICard card, ICard other, ref float myDamage, ref float otherDamage)
        {
            // missed 
            if (other is MonsterCard { MonsterType: MonsterType.Elf, Element: Element.Fire }) myDamage = 0;
        }
    }
}