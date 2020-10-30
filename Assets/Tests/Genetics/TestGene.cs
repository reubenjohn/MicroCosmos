using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

public class TestGene : IGene<TestGene>
{
    public float furryness { get; }
    private static readonly Mutator.ClampedFloat FURRINESS_MUTATOR = new Mutator.ClampedFloat(0.1f, 0f, 1f);
    public uint nEyes { get; }
    public DietaryRestriction dietaryRestriction { get; }
    public Limb[] limbs { get; }

    public TestGene(float furryness, uint nEyes, DietaryRestriction dietaryRestriction, Limb[] limbs)
    {
        this.furryness = furryness;
        this.nEyes = nEyes;
        this.dietaryRestriction = dietaryRestriction;
        this.limbs = limbs;
    }

    public override TestGene Deserialize(string dnaSequence)
    {
        return JsonConvert.DeserializeObject<TestGene>(dnaSequence);
    }

    public override TestGene Mutate()
    {
        return new TestGene(
            FURRINESS_MUTATOR.Mutate(furryness),
            nEyes.Mutate(.1f),
            dietaryRestriction.Mutate(.1f),
            limbs.Mutate(Limb.MUTATOR)
        );
    }

    public override string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }

    // public override TestGene Duplicate()
    // {
    //     return new TestGene(furryness, nEyes, dietaryRestriction, limbs);
    // }

    public override bool Equals(object obj)
    {
        return obj is TestGene gene &&
               furryness == gene.furryness &&
               nEyes == gene.nEyes &&
               dietaryRestriction == gene.dietaryRestriction &&
               (limbs == null) == (gene.limbs == null) &&
               (limbs == null || Enumerable.SequenceEqual(limbs, gene.limbs));
    }

    public override int GetHashCode()
    {
        int hashCode = -362057520;
        hashCode = hashCode * -1521134295 + furryness.GetHashCode();
        hashCode = hashCode * -1521134295 + nEyes.GetHashCode();
        hashCode = hashCode * -1521134295 + dietaryRestriction.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<Limb[]>.Default.GetHashCode(limbs);
        return hashCode;
    }

    public override string ToString()
    {
        return Serialize();
    }

    public enum DietaryRestriction
    {
        HERBIVORE, CARNIVORE
    }

    public class Limb : IGene<Limb>
    {
        public static readonly Mutator.ClampedFloat LENTH_MUTATOR = new Mutator.ClampedFloat(.1f, 0f, float.MaxValue);
        public static readonly System.Func<Limb, Limb> MUTATOR = limb => limb.Mutate();

        public float length { get; private set; }

        public Limb(float length)
        {
            this.length = length;
        }

        public override string Serialize()
        {
            return JsonUtility.ToJson(this);
        }

        public override Limb Deserialize(string sequence)
        {
            return JsonUtility.FromJson<Limb>(sequence);
        }

        // public override Limb Duplicate()
        // {
        //     return new Limb(length);
        // }

        public override Limb Mutate()
        {
            return new Limb(LENTH_MUTATOR.Mutate(length));
        }

        public override bool Equals(object obj)
        {
            return obj is Limb limb &&
                   length == limb.length;
        }

        public override int GetHashCode()
        {
            return -1136221603 + length.GetHashCode();
        }
    }
}
