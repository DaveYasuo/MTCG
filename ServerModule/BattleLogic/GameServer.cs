using System.Collections.Generic;
using ServerModule.Database.Models;

namespace ServerModule.BattleLogic
{
    public class GameServer : IServer
    {
        public static readonly GameServer Instance = new();

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        public bool QueuePlayer(string username, List<Card> cards)
        {
            throw new System.NotImplementedException();
        }
    }
}