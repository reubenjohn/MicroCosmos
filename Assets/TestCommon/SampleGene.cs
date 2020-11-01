using System;
using System.Collections.Generic;
using System.Linq;
using Genetics;
using Newtonsoft.Json;

namespace Tests.Genetics
{
    public class SampleGene
    {
        public const string Serialized1 =
            @"{""furriness"":0.5,""nEyes"":2,""dietaryRestriction"":0,""limbs"":[{""length"":3.4},{""length"":3.4}]}";

        public SampleGene(float furriness, uint nEyes, DietaryRestriction dietaryRestriction, Limb[] limbs)
        {
            this.furriness = furriness;
            this.nEyes = nEyes;
            this.dietaryRestriction = dietaryRestriction;
            this.limbs = limbs;
        }

        public float furriness { get; }
        public uint nEyes { get; }
        public DietaryRestriction dietaryRestriction { get; }
        public Limb[] limbs { get; }

        public override bool Equals(object obj)
        {
            return obj is SampleGene gene &&
                   Math.Abs(furriness - gene.furriness) < 1e-5 &&
                   nEyes == gene.nEyes &&
                   dietaryRestriction == gene.dietaryRestriction &&
                   (limbs == gene.limbs || limbs != null && gene.limbs != null && limbs.SequenceEqual(gene.limbs));
        }

        public override int GetHashCode()
        {
            var hashCode = -362057520;
            hashCode = hashCode * -1521134295 + furriness.GetHashCode();
            hashCode = hashCode * -1521134295 + nEyes.GetHashCode();
            hashCode = hashCode * -1521134295 + dietaryRestriction.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Limb[]>.Default.GetHashCode(limbs);
            return hashCode;
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }

    public enum DietaryRestriction
    {
        Herbivore,
        Carnivore
    }

    public class Limb
    {
        public static readonly Mutator.ClampedFloat LengthMutator = new Mutator.ClampedFloat(.1f, 0f, float.MaxValue);
        public static readonly Func<Limb, Limb> Mutator = Mutate;

        public Limb(float length)
        {
            this.length = length;
        }

        public float length { get; }

        public static Limb Mutate(Limb limb) => new Limb(LengthMutator.Mutate(limb.length));

        public override bool Equals(object obj)
        {
            return obj is Limb limb &&
                   Math.Abs(length - limb.length) < 1e-5;
        }

        public override int GetHashCode()
        {
            return -1136221603 + length.GetHashCode();
        }
    }
}