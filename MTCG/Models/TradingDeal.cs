using System;
using MTCG.Data.Cards.Types;

namespace MTCG.Models
{
    public class TradingDeal
    {
        public TradingDeal(Guid id, Guid cardToTrade, string type, float minimumDamage)
        {
            Id = id;
            CardToTrade = cardToTrade;
            Type = type;
            MinimumDamage = minimumDamage;
        }

        public Guid Id { get; }
        public Guid CardToTrade { get; }
        public string Type { get; }
        public float MinimumDamage { get; }

        public CardType GetCardType()
        {
            return Type.ToLower() switch
            {
                "monster" => CardType.Monster,
                "spell" => CardType.Spell,
                _ => throw new ArgumentOutOfRangeException(Type + " is not a valid type")
            };
        }
    }
}