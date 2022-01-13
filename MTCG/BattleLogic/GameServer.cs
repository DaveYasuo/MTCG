using System;
using System.Collections.Generic;
using ServerModule;
using ServerModule.Models;

namespace MTCG.BattleLogic
{
    public class GameServer : IServer
    {
        public GameServer()
        {
            Console.WriteLine("Game");
        }
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