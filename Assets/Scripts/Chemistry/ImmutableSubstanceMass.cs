using System;
using UnityEngine;

namespace Chemistry
{
    [Serializable]
    public class ImmutableSubstanceMass<T> where T : Enum
    {
        [SerializeField] public readonly T substance;
        [SerializeField] public readonly float mass;

        public ImmutableSubstanceMass(T substance, float mass)
        {
            this.substance = substance;
            this.mass = mass;
        }

        // public SubstanceMass<T> ToMutable() => new SubstanceMass<T> {substance = substance, mass = mass};
    }
}