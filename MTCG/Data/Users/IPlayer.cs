using System.Collections.Generic;
using MTCG.Data.Cards;

namespace MTCG.Data.Users
{
    public interface IPlayer
    {
        public string Username { get; }
        public List<ICard> Cards { get; }
        public List<object> Log { get; }
        public  bool InGame { get; set; }
        public string LastPlayedCard { get; set; }

        /// <summary>
        /// Generate a random index of the card.length and returns the card on that position
        /// </summary>
        /// <returns>A random card from the player's own deck</returns>
        ICard GetRandomCard();

        /// <summary>
        /// add the specific card to one's deck
        /// </summary>
        /// <param name="card"></param>
        void Add(ICard card);

        /// <summary>
        /// Remove the specific card from one's deck
        /// </summary>
        /// <param name="card"></param>
        void Remove(ICard card);
    }
}