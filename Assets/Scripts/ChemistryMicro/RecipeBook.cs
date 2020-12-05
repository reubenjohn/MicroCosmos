using System.Collections.Generic;
using Chemistry;

namespace ChemistryMicro
{
    public enum Recipe
    {
        NOP,
        GrowSkin,
        AgeSkin
    }

    public class RecipeBook
    {
        public static readonly RecipeBook Singleton = new RecipeBook();

        private readonly Dictionary<Recipe, Reaction<Substance>> recipes = new Dictionary<Recipe, Reaction<Substance>>
        {
            //No-Operation
            [Recipe.NOP] = Substance.Waste.M(0) > Substance.Waste.M(0),

            // Vitals
            [Recipe.GrowSkin] =
                Substance.SkinGrowthFactor.M(1) + Substance.Fat.M(1) >
                Substance.SkinGrowthFactor.M(1) + Substance.Skin.M(.5) + Substance.Waste.M(.5),
            [Recipe.AgeSkin] =
                Substance.SkinGrowthFactor.M(1) + Substance.Skin.M(.01) >
                Substance.SkinGrowthFactor.M(1) + Substance.Waste.M(.01)
        };

        public readonly Recipe[] voluntaryRecipes =
        {
            Recipe.GrowSkin,
            Recipe.NOP,
            Recipe.NOP,
            Recipe.NOP,
            Recipe.NOP,
            Recipe.NOP,
            Recipe.NOP,
            Recipe.NOP,
            Recipe.NOP,
            Recipe.NOP,
            Recipe.NOP,
            Recipe.NOP,
            Recipe.NOP,
            Recipe.NOP,
            Recipe.NOP,
            Recipe.NOP
        };

        public Reaction<Substance> this[Recipe key] => recipes[key];
    }
}