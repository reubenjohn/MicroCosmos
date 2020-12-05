using System;

namespace Chemistry
{
    public class Reaction<T> where T : Enum
    {
        public readonly Mixture<T> ingredients;
        public readonly Mixture<T> effects;
        public readonly Mixture<T> change;

        public Reaction(Mixture<T> ingredients, Mixture<T> effects)
        {
            this.ingredients = ingredients;
            this.effects = effects;
            change = effects - ingredients;
            if (Math.Abs(change.TotalMass) > 1e-6)
                throw new ArgumentException(
                    "Mass must be conserved in a reaction " +
                    $"[{change.TotalMass}][{change}]({ingredients.TotalMass.ToString()} != " +
                    $"{effects.TotalMass.ToString()}):\n{ToString()}"
                );
        }

        // public Reaction(MixtureDictionary<T> ingredients, MixtureDictionary<T> effects) : this(ingredients.ToMixture(),
        //     effects.ToMixture())
        // {
        // }

        public override string ToString()
        {
            return ingredients + " -> " + effects;
        }
    }
}