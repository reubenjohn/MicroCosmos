using System.Collections.Generic;
using Chemistry;

namespace ChemistryMicro
{
    public enum Recipe
    {
        Nop,
        GrowSkin,
        AgeSkin,
        Recycle
    }

    public class RecipeBook
    {
        public static readonly RecipeBook Singleton = new RecipeBook();
        public static readonly float Density = .1f;

        private readonly Dictionary<Recipe, Reaction<Substance>> recipes = new Dictionary<Recipe, Reaction<Substance>>
        {
            //No-Operation
            [Recipe.Nop] = Substance.Waste.M(0) > Substance.Waste.M(0),

            // Vitals
            [Recipe.GrowSkin] =
                Substance.SkinGrowthFactor.M(1) + Substance.Fat.M(1) >
                Substance.SkinGrowthFactor.M(1) + Substance.Skin.M(.5) + Substance.Waste.M(.5),
            [Recipe.AgeSkin] =
                Substance.SkinAgeFactor.M(1f) + Substance.Skin.M(1f) >
                Substance.SkinAgeFactor.M(1.01f) + Substance.Waste.M(.99),

            [Recipe.Recycle] =
                Substance.Waste.M(1) > Substance.Fat.M(1)
        };

        public Reaction<Substance> this[Recipe key] => recipes[key];
    }
}