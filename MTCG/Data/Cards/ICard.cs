using MTCG.Data.Cards.Types;
using System.Collections.Generic;
using MTCG.Data.Cards.Specialties;

namespace MTCG.Data.Cards
{
    public interface ICard
    {
        string Name { get; }
        float Damage { get; }
        Element Element { get; }

        // Using IEnumerable
        // See: https://stackoverflow.com/a/30646094
        IEnumerable<ISpecialty> Specialties { get; }
    }
}