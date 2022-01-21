using System;
using System.Collections.Generic;
using MTCG.Data.Cards;

namespace MTCG.Data.Users
{
    /// <inheritdoc cref="IPlayer"/>
    public class Player : IPlayer
    {
        public string Username { get; }
        public List<ICard> Cards { get; }
        public List<object> Log { get; set; }
        public bool InGame { get; set; }
        /// <summary>
        /// Gets the name of the last played card
        /// </summary>
        public string LastPlayedCard { get; set; }

        private readonly Random _rnd;

        public ICard GetRandomCard()
        {
            ICard card = GenerateRandomCard();
            LastPlayedCard = card.Name;
            return card;
        }

        public void Add(ICard card)
        {
            Cards.Add(card);
        }

        public void Remove(ICard card)
        {
            Cards.Remove(card);
        }

        public Player(string username, List<ICard> cards, bool inGame = false)
        {
            Username = username;
            Cards = cards;
            Log = new List<object>();
            InGame = inGame;
            _rnd = new Random();
        }

        private ICard GenerateRandomCard()
        {
            // Random number with max value of the cards in the list
            // See: https://docs.microsoft.com/en-us/dotnet/api/system.random.next?view=net-6.0
            return Cards[_rnd.Next(Cards.Count)];
        }
    }
}