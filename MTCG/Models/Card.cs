using System;

namespace MTCG.Models
{
    public class Card
    {
        public Guid Id { get; }
        public string Name { get; }
        public float Damage { get; }

        public Card(Guid id, string name, float damage)
        {
            Id = id;
            Name = name;
            Damage = damage;
        }
    }
}