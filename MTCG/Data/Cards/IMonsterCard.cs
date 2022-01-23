using MTCG.Data.Cards.Types;

namespace MTCG.Data.Cards
{
    public interface IMonsterCard : ICard
    {
        public MonsterType MonsterType { get; }
    }
}