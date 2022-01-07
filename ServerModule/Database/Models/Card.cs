namespace ServerModule.Database.Models
{
    public class Card
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Damage { get; set; }

        public Card(string id, string name, double damage)
        {
            Id = id;
            Name = name;
            Damage = damage;
        }
    }
}