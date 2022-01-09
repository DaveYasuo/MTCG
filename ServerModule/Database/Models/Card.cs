using System;

namespace ServerModule.Database.Models
{
    public class Card
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Damage { get; set; }

        public Card(Guid id, string name, double damage)
        {
            Id = id;
            Name = name;
            Damage = damage;
        }
    }
}