using System.Collections.Generic;
using MTCG.Models;

namespace MTCG.Data.Users
{
    public interface IPlayer
    {
        public string Username { get; }
        public List<Card> Cards { get; }
        public List<string> Log { get; }
        public  bool InGame { get; set; }
    }
}