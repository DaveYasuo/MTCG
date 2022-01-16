using MTCG.Models;
using System.Collections.Generic;

namespace MTCG.Data.Users
{
    public class Player : IPlayer
    {
        public string Username { get; }
        public List<Card> Cards { get; }
        public List<string> Log { get; }
        public bool InGame { get; set; }

        public Player(string username, List<Card> cards, List<string> log = null, bool inGame = false)
        {
            Username = username;
            Cards = cards;
            Log = log;
            InGame = inGame;
        }
    }
}