using System;

namespace Chemistry
{
    public interface IFlaskBehavior<in T> where T : Enum
    {
        float TotalMass { get; }
        int Length { get; }
        float this[T substance] { get; }
    }
}