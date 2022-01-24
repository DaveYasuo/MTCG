using System.Collections.Generic;
using MTCG.Data.Cards.Specialties;
using MTCG.Data.Cards.Types;

namespace MTCG.Data.Cards
{
    public interface ICard
    {
        public string Name { get; }
        public float Damage { get; }

        public Element Element { get; }

        // Using IEnumerable
        // See: https://stackoverflow.com/a/30646094
        public IEnumerable<ISpecialty> Specialties { get; }
    }
}