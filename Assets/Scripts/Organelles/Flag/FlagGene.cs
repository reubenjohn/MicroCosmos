using System;

namespace Organelles.Flag
{
    [Serializable]
    public class FlagGene
    {
        public float hue = 0.5f;

        public FlagGene(float hue)
        {
            this.hue = hue;
        }
    }
}