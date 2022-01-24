using System;

namespace MTCG.Models
{
    public class Card
    {
        public Card(Guid id, string name, float damage)
        {
            Id = id;
            Name = name;
            Damage = damage;
        }

        public Guid Id { get; }
        public string Name { get; set; }
        public float Damage { get; }
    }
}