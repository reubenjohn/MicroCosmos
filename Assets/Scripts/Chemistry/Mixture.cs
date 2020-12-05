using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chemistry
{
    public class Mixture<T> where T : Enum
    {
        public readonly float[] contents;
        // private readonly Lazy<float> totalMass;

        private int EnumCount() => Enum.GetValues(typeof(T)).Length;

        public Mixture()
        {
            contents = new float[EnumCount()];
            // totalMass = new Lazy<float>(Mass);
        }

        public Mixture(IDictionary<T, float> source) : this()
        {
            foreach (var pair in source)
                contents[Convert.ToInt32(pair.Key)] = pair.Value;
        }

        private Mixture(float[] contents) : this()
        {
            this.contents = new float[contents.Length];
            Array.Copy(contents, this.contents, this.contents.Length);
        }

        protected Mixture(Mixture<T> initialMix) : this(initialMix.contents)
        {
        }

        // [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public float TotalMass => Mass();

        public float Mass() => contents.Sum();

        public Mixture<T> Copy() => new Mixture<T>(contents);

        public override string ToString() =>
            $"{{ {string.Join(", ", Entries().Select(entry => $"{entry.Key}: {entry.Value}"))} }}";

        private IEnumerable<KeyValuePair<T, float>> Entries()
        {
            for (var i = 0; i < contents.Length; i++)
                if (!Mathf.Approximately(contents[i], 0))
                    yield return new KeyValuePair<T, float>((T) Enum.ToObject(typeof(T), i), contents[i]);
        }

        protected bool Equals(Mixture<T> other)
        {
            for (var i = 0; i < contents.Length; i++)
                if (!Mathf.Approximately(contents[i], other.contents[i]))
                    return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Mixture<T>) obj);
        }

        public override int GetHashCode() => throw new NotImplementedException();

        public static Mixture<T> operator +(Mixture<T> a, Mixture<T> b) =>
            new Mixture<T>(a.contents.Zip(b.contents, (af, bf) => af + bf).ToArray());

        public static Mixture<T> operator -(Mixture<T> a, Mixture<T> b) =>
            new Mixture<T>(a.contents.Zip(b.contents, (af, bf) => af - bf).ToArray());

        public static Mixture<T> operator *(Mixture<T> a, float scale) =>
            new Mixture<T>(a.contents.Select(x => x * scale).ToArray());
    }

    public static class MixtureUtils
    {
        public static Mixture<T> ToMixture<T>(this MixtureDictionary<T> mixDict) where T : Enum =>
            new Mixture<T>(mixDict);
    }
}