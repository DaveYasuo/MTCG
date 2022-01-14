using System;
using System.Collections.Generic;
using MTCG.Models;
using ServerModule;

namespace MTCG.BattleLogic
{
    public class GameServer : IServer
    {
        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public bool QueuePlayer(string username, List<Card> cards)
        {
            throw new NotImplementedException();
        }

        public string Play(string username, List<Card> cards)
        {
            return "";
        }
    }
}