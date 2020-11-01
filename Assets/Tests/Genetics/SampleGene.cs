﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class SampleGene
{
    public SampleGene(float furryness, uint nEyes, DietaryRestriction dietaryRestriction, Limb[] limbs)
    {
        this.furryness = furryness;
        this.nEyes = nEyes;
        this.dietaryRestriction = dietaryRestriction;
        this.limbs = limbs;
    }

    public float furryness { get; }
    public uint nEyes { get; }
    public DietaryRestriction dietaryRestriction { get; }
    public Limb[] limbs { get; }

    public override bool Equals(object obj)
    {
        return obj is SampleGene gene &&
               Math.Abs(furryness - gene.furryness) < 1e-5 &&
               nEyes == gene.nEyes &&
               dietaryRestriction == gene.dietaryRestriction &&
               (limbs == gene.limbs || limbs != null && gene.limbs != null && limbs.SequenceEqual(gene.limbs));
    }

    public override int GetHashCode()
    {
        var hashCode = -362057520;
        hashCode = hashCode * -1521134295 + furryness.GetHashCode();
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
    public static readonly Mutator.ClampedFloat LenthMutator = new Mutator.ClampedFloat(.1f, 0f, float.MaxValue);
    public static readonly Func<Limb, Limb> Mutator = Mutate;

    public Limb(float length)
    {
        this.length = length;
    }

    public float length { get; }

    public static Limb Mutate(Limb limb) => new Limb(LenthMutator.Mutate(limb.length));

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