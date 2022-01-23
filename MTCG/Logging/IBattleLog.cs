using System.Collections.Generic;
using DebugAndTrace;
using MTCG.BattleLogic;

namespace MTCG.Logging
{
    public interface IBattleLog
    {
        public List<object> LogList { get; }
        public void AddRound(float damage1, float damage2, int round);
        public BattleResult GetResult();
        void AddStartInfo();
        void AddBaseDamage(float myDamage, float otherDamage);
        void AddElementReaction(string myElement, string otherElement);
        void AddEffectInfo(string infoMessage);
        void AddEffectiveDamage(float myDamage, float otherDamage);
        void AddBonusDamage(int bonusDmg1, int bonusDmg2);
    }
}