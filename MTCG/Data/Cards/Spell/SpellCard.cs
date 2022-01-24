using System.Collections.Generic;
using MTCG.Data.Cards.Specialties;
using MTCG.Data.Cards.Types;

namespace MTCG.Data.Cards.Spell
{
    public class SpellCard : ISpellCard
    {
        public string Name { get; }
        public float Damage { get; set; }
        public Element Element { get; }
        public IEnumerable<ISpecialty> Specialties { get; }

        public SpellCard(string name, float damage, Element element, IEnumerable<ISpecialty> specialties)
        {
            Name = name;
            Damage = damage;
            Element = element;
            Specialties = specialties;
        }
    }
}