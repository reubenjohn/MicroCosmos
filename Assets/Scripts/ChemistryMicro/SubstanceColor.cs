using Chemistry;
using UnityEngine;
using Util;

namespace ChemistryMicro
{
    public class SubstanceColor
    {
        private static readonly int NSubstances;
        private static readonly Color[] SubstanceColors;

        static SubstanceColor()
        {
            NSubstances = EnumUtils.EnumCount(typeof(Substance));
            SubstanceColors = new[]
            {
                Color.magenta, //Fat,
                Color.HSVToRGB(.1f, .5f, .35f), //Waste
                Color.gray, //SkinGrowthFactor
                Color.blue //Skin
            };
        }

        public static Color ColorOf(Substance substance) => SubstanceColors[(int) substance];

        public static Color ColorOf(Mixture<Substance> mix)
        {
            var initMass = mix.contents[0];
            var sum = initMass;
            var color = initMass * SubstanceColors[0];
            for (var i = 1; i < NSubstances; i++)
            {
                var mass = mix.contents[i];
                color += mass * SubstanceColors[i];
                sum += mass;
            }

            if (sum == 0)
                return Color.gray;

            return color / sum;
        }
    }
}