using MTCG.Data.Cards.Types;

namespace MTCG.Data.Cards.Specialties
{
    public class ControlOrk : ISpecialty
    {
        public void ApplyEffect(ICard card, ICard other, ref float myDamage, ref float otherDamage)
        {
            // enemy missed
            if (other is MonsterCard { MonsterType: MonsterType.Ork }) otherDamage = 0;
        }
    }
}