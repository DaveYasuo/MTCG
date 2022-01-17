using MTCG.Data.Cards.Types;

namespace MTCG.Data.Cards.Specialties
{
    public class AfraidOfDragons : ISpecialty
    {
        public void ApplyEffect(ICard card, ICard other, ref float myDamage, ref float otherDamage)
        {
            // miss the attack
            if (other is MonsterCard { MonsterType: MonsterType.Dragon }) myDamage = 0;
        }
    }
}