using System;

namespace MTCG.Models
{
    public class CardWithUsername : Card
    {
        public string Username { get; }

        public CardWithUsername(Guid id, string cardName, float damage, string username) : base(id, cardName, damage)
        {
            Username = username;
        }
    }
}