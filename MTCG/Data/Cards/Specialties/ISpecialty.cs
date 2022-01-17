using MTCG.Data.Utils;

namespace MTCG.Data.Cards.Specialties
{
    public interface ISpecialty
    {
        void ApplyEffect(ICard card, ICard other, ref float myDamage, ref float otherDamage);
    }
}