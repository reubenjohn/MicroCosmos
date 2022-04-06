using System;

namespace Organelles.HunterFlag
{
    [Serializable]
    public class HunterFlagGene
    {
        public float hue = 0.5f;

        public HunterFlagGene(float hue)
        {
            this.hue = hue;
        }
    }
}