using System;

namespace Brains
{
    [Serializable]
    public class GeneticBrainGene
    {
        public float[,] weights;
        public float[] biases;
    }
}