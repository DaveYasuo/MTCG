using MTCG.Models;
using System.Collections.Generic;

namespace MTCG.Data.Users
{
    public class Player : IPlayer
    {
        public string Username { get; }
        public List<Card> Cards { get; }

        public Player(string username)
        {
            Username = username;
        }

        public Player(string username, List<Card> cards) : this(username)
        {
            Cards = cards;
        }
    }
}