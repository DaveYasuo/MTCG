using System.Collections.Generic;
using MTCG.Data.Cards.Specialties;
using MTCG.Data.Cards.Types;
using MTCG.Models;

namespace MTCG.Data.Cards
{
    public class SpellCard : ICard
    {
        public string Name { get; }
        public float Damage { get; set; }
        public Element Element { get; }
        public IEnumerable<ISpecialty> Specialties { get; }

        public SpellCard(Card card, Element element, IEnumerable<ISpecialty> specialties)
        {
            Name = card.Name;
            Damage = card.Damage;
            Element = element;
            Specialties = specialties;
        }
    }
}