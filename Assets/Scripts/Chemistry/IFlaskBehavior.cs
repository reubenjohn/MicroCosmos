using System;

namespace Chemistry
{
    public interface IFlaskBehavior<T> where T : Enum
    {
        float TotalMass { get; }
        int Length { get; }
        float this[T substance] { get; }
        Mixture<T> ToMixture();
    }
}