using System.Collections.Generic;
using MTCG.Data.Cards.Specialties;
using MTCG.Data.Cards.Types;

namespace MTCG.Data.Cards.Monster
{
    public class MonsterCard : IMonsterCard
    {
        public MonsterCard(string name, float damage, MonsterType monsterType, Element element,
            IEnumerable<ISpecialty> specialties)
        {
            Name = name;
            Damage = damage;
            MonsterType = monsterType;
            Element = element;
            Specialties = specialties;
        }

        public string Name { get; }
        public float Damage { get; set; }
        public MonsterType MonsterType { get; }
        public Element Element { get; }
        public IEnumerable<ISpecialty> Specialties { get; }
    }
}