using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public static class Mutator
{
    // public static readonly float SIGMA = 0.68f;
    // public static float Normal(float x, float mean, float std)
    // {
    //     return Mathf.Exp(-Mathf.Pow((x - mean) / std, 2) / 2) / (std * Mathf.Sqrt(2 * Mathf.PI));
    // }

    public interface IMutator<T>
    {
        T Mutate(T val);
    }

    public class Float : IMutator<float>
    {
        public float mutationRate { get; }

        public static float Mutate(float val, float mutationRate)
        {
            return Random.Range(val - mutationRate, val + mutationRate);
        }

        public Float(float mutationRate)
        {
            this.mutationRate = mutationRate;
        }

        public float Mutate(float logit)
        {
            return Mutate(logit, mutationRate);
        }
    }

    public static float Mutate(this float val, float mutationRate) => Float.Mutate(val, mutationRate);

    public class ClampedFloat : Mutator.Float
    {
        public float min { get; }
        public float max { get; }

        public static float Mutate(float logit, float mutationRate, float min, float max)
        {
            var unclamped = Mutator.Float.Mutate(logit, mutationRate);
            return Mathf.Clamp(unclamped, min, max);
        }

        public ClampedFloat(float mutationRate, float min, float max) : base(mutationRate)
        {
            this.min = min;
            this.max = max;
        }

        public new float Mutate(float logit)
        {
            return Mutate(logit, mutationRate);
        }
    }

    public static float MutateClamped(this float val, float mutationRate, float min, float max) =>
        ClampedFloat.Mutate(val, mutationRate, min, max);

    public class Int : IMutator<int>
    {
        public float mutationRate { get; }

        public Int(float mutationRate)
        {
            this.mutationRate = mutationRate;
        }

        public int Mutate(int count)
        {
            return Mutate(count, mutationRate);
        }

        public static int Mutate(int val, float mutationRate)
        {
            var ceil = Mathf.Ceil(mutationRate);
            var sample = Random.Range(-ceil, ceil);
            sample += Mathf.Sign(sample) * mutationRate;
            return val + (int) sample;
        }
    }

    public static int Mutate(this int val, float mutationRate) => Int.Mutate(val, mutationRate);

    public class ClampedInt : Int
    {
        public int min { get; }
        public int max { get; }

        public ClampedInt(float mutationRate, int min, int max) : base(mutationRate)
        {
            this.min = min;
            this.max = max;
        }

        public new int Mutate(int val)
        {
            return Mutate(val, mutationRate, min, max);
        }

        public static int Mutate(int val, float mutationRate, int min, int max)
        {
            var mutated = Mutator.Int.Mutate(val, mutationRate);
            return Mathf.Clamp(mutated, min, max);
        }
    }

    public static int MutateClamped(this int val, float mutationRate, int min, int max) =>
        ClampedInt.Mutate(val, mutationRate, min, max);

    public class UnsignedInt : IMutator<uint>
    {
        public float mutationRate { get; }

        public UnsignedInt(float mutationRate)
        {
            this.mutationRate = mutationRate;
        }

        public uint Mutate(uint val)
        {
            return Mutate(val, mutationRate);
        }

        public static uint Mutate(uint val, float mutationRate)
        {
            var ceil = Mathf.Ceil(mutationRate);
            var sample = Random.Range(-ceil, ceil);
            sample += Mathf.Sign(sample) * mutationRate;
            sample = Mathf.Clamp(sample, 0, uint.MaxValue);
            return val + (uint) sample;
        }
    }

    public static uint Mutate(this uint val, float mutationRate) => UnsignedInt.Mutate(val, mutationRate);

    public class Enum
    {
        public float mutationRate { get; }

        public Enum(float mutationRate)
        {
            this.mutationRate = mutationRate;
        }

        public T Mutate<T>(T enumeration) where T : System.Enum
        {
            return Mutate(enumeration, this.mutationRate);
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public static T Mutate<T>(T enumeration, float mutationRate) where T : System.Enum
        {
            var val = System.Convert.ToInt32(enumeration);
            var vals = System.Enum.GetValues(typeof(T)).Cast<int>();
            var min = vals.FirstOrDefault();
            var max = vals.LastOrDefault();
            var mutated = (uint) ClampedInt.Mutate(val, mutationRate, min, max);
            return (T) System.Enum.ToObject(typeof(T), mutated);
        }
    }

    public static T Mutate<T>(this T val, float mutationRate) where T : System.Enum => Enum.Mutate(val, mutationRate);

    public class Enum<T> : IMutator<T> where T : System.Enum
    {
        public float mutationRate { get; private set; }
        private readonly uint min, max;

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public Enum(float mutationRate)
        {
            this.mutationRate = mutationRate;
            var vals = System.Enum.GetValues(typeof(T)).Cast<uint>();
            min = vals.FirstOrDefault();
            max = vals.LastOrDefault();
        }

        public T Mutate(T enumeration)
        {
            var val = System.Convert.ToInt32(enumeration);
            var mutated = Int.Mutate(val, mutationRate);
            var clamped = (uint) Mathf.Clamp(mutated, this.min, this.max);
            return (T) System.Enum.ToObject(typeof(T), clamped);
        }
    }

    public class MutationEnumerable<T> : IMutator<IEnumerable<T>>
    {
        private readonly System.Func<T, T> elementMutation;

        public MutationEnumerable(System.Func<T, T> elementMutation)
        {
            this.elementMutation = elementMutation;
        }

        public IEnumerable<T> Mutate(IEnumerable<T> enumerable)
        {
            return enumerable.Select(elementMutation);
        }

        public static IEnumerable<T> Mutate(IEnumerable<T> enumerable, System.Func<T, T> elementMutation)
        {
            return enumerable.Select(elementMutation);
        }
    }

    public static IEnumerable<T> Mutate<T>(this IEnumerable<T> val, System.Func<T, T> elementMutation) =>
        MutationEnumerable<T>.Mutate(val, elementMutation);

    public static T[] Mutate<T>(this T[] val, System.Func<T, T> elementMutation) =>
        MutationEnumerable<T>.Mutate(val.AsEnumerable(), elementMutation).ToArray();

    public class MutatorEnumerable<T> : MutationEnumerable<T>
    {
        public MutatorEnumerable(IMutator<T> elementMutator) : base(elementMutator.Mutate)
        {
        }

        public static IEnumerable<T> Mutate(IEnumerable<T> enumerable, IMutator<T> elementMutator)
        {
            return enumerable.Select(elementMutator.Mutate);
        }
    }

    public static IEnumerable<T> Mutate<T>(this IEnumerable<T> val, IMutator<T> elementMutator) =>
        MutatorEnumerable<T>.Mutate(val, elementMutator);
}