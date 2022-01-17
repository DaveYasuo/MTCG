namespace MTCG.Data.Cards.Specialties
{
    public class ImmunAgainstSpells : ISpecialty
    {
        public void ApplyEffect(ICard card, ICard other, ref float myDamage, ref float otherDamage)
        {
            if (other is SpellCard) otherDamage = 0;
        }
    }
}