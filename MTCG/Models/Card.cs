using System;

namespace MTCG.Models
{
    public class Card
    {
        public Guid Id { get; }
        public string Name { get; }
        public double Damage { get; }

        public Card(Guid id, string name, double damage)
        {
            Id = id;
            Name = name;
            Damage = damage;
        }
    }
}