using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chemistry
{
    public class Flask<T> : Mixture<T> where T : Enum
    {
        public Flask()
        {
        }

        public Flask(IDictionary<T, float> initialMix) : base(initialMix)
        {
        }

        public Flask(Mixture<T> initialMix) : base(initialMix)
        {
        }

        private bool Take(Mixture<T> b)
        {
            if (!MassesGreaterThanEqualTo(b))
                return false;
            for (var i = 0; i < contents.Length; i++)
                contents[i] -= b.contents[i];
            return true;
        }

        private void Put(Mixture<T> b)
        {
            for (var i = 0; i < contents.Length; i++)
                contents[i] += b.contents[i];
        }

        private bool MassesGreaterThanEqualTo(Mixture<T> other) =>
            contents.Zip(other.contents, (x, y) => x >= y || Mathf.Approximately(x, y)).All(b => b);


        private static float MaxYield(Mixture<T> available, Mixture<T> required) =>
            available.contents
                .Zip(required.contents, (av, req) => req != 0 ? av / req : float.MaxValue)
                .Min();

        public float Convert(Reaction<T> reaction, float conversionFactor = 1f)
        {
            if (conversionFactor < 0 || conversionFactor > 1f)
                throw new InvalidOperationException(
                    $"Reactions conversion factor '{conversionFactor}' must lie in range [0, 1]");
            var yield = MaxYield(this, reaction.ingredients) * conversionFactor;

            if (yield > 0)
            {
                Take(reaction.ingredients * yield);
                Put(reaction.effects * yield);
            }

            return yield;
        }

        public static bool TryTransfer(Flask<T> destination, Flask<T> source, Mixture<T> transferMixture)
        {
            // ReSharper disable once PossibleUnintendedReferenceComparison
            if (source == transferMixture)
                transferMixture = transferMixture.Copy();

            if (source.Take(transferMixture))
            {
                destination.Put(transferMixture);
                return true;
            }

            return false;
        }

        public static void Transfer(Flask<T> destination, Flask<T> source, Mixture<T> transferMixture)
        {
            if (!TryTransfer(destination, source, transferMixture))
                throw new InvalidOperationException(
                    $"Insufficient substance in source. Required {transferMixture} but source was {source}");
        }
    }

    public static class FlaskUtils
    {
        public static Flask<T> ToFlask<T>(this MixtureDictionary<T> mixDict) where T : Enum => new Flask<T>(mixDict);

        public static Flask<T> ToFlask<T>(this Mixture<T> mix) where T : Enum => new Flask<T>(mix);
    }
}