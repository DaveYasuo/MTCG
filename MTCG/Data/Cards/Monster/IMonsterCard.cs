using MTCG.Data.Cards.Types;

namespace MTCG.Data.Cards.Monster
{
    public interface IMonsterCard : ICard
    {
        public MonsterType MonsterType { get; }
    }
}