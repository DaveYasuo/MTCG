using System.Collections.Generic;
using ServerModule;
using ServerModule.Models;

namespace MTCG.BattleLogic
{
    public class GameServer : IServer
    {
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

        public string Play(string username, List<Card> cards)
        {
            return "";
        }
    }
}