using System.Collections.Generic;
using MTCG.Data.Cards.Specialties;
using MTCG.Data.Cards.Types;
using MTCG.Models;

namespace MTCG.Data.Cards
{
    public class MonsterCard : IMonsterCard
    {
        public string Name { get; }
        public float Damage { get; set; }
        public MonsterType MonsterType { get; }
        public Element Element { get; }
        public IEnumerable<ISpecialty> Specialties { get; }

        public MonsterCard(Card card, MonsterType monsterType, Element element, IEnumerable<ISpecialty> specialties)
        {
            Name = card.Name;
            Damage = card.Damage;
            MonsterType = monsterType;
            Element = element;
            Specialties = specialties;
        }
    }
}