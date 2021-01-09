using System;
using System.Linq;
using Chemistry;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace ChemistryMicro
{
    public class SubstanceColor
    {
        private static readonly int NSubstances;
        private static readonly Color[] SubstanceColors;
        public static readonly NamedColor[] NamedColors;

        static SubstanceColor()
        {
            NSubstances = EnumUtils.EnumCount(typeof(Substance));
            SubstanceColors = new Color[NSubstances];
            for (var i = 0; i < SubstanceColors.Length; i++)
                SubstanceColors[i] = Color.HSVToRGB(Random.Range(0f, 1f), 1, .5f);
            SubstanceColors[(int) Substance.Fat] = Color.magenta;
            SubstanceColors[(int) Substance.Waste] = Color.HSVToRGB(.1f, .5f, .35f);
            SubstanceColors[(int) Substance.Skin] = Color.blue;
            SubstanceColors[(int) Substance.SkinGrowthFactor] = Color.gray;
            SubstanceColors[(int) Substance.SkinAgeFactor] = Color.black;
            NamedColors = Enum.GetValues(typeof(Substance)).Cast<Substance>()
                .Select((substance, index) =>
                    new NamedColor(index, substance, substance.ToString(), SubstanceColors[index]))
                .ToArray();
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
                return Color.black;

            return color / sum;
        }
    }

    public class NamedColor
    {
        public readonly Color color;
        public readonly int index;
        public readonly string name;
        public readonly Substance substance;

        public NamedColor(int index, Substance substance, string name, Color color)
        {
            this.index = index;
            this.substance = substance;
            this.name = name;
            this.color = color;
        }
    }
}