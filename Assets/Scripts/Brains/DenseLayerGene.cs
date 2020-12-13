using System;
using Newtonsoft.Json;

namespace Brains
{
    [Serializable]
    public class DenseLayerGene
    {
        [JsonConstructor]
        public DenseLayerGene(float[,] weights, float[] biases)
        {
            if (weights.GetLength(0) != biases.Length)
                throw new ArgumentException(
                    $"weights shape [{weights.GetLength(0)}, {weights.GetLength(1)}] " +
                    $"must be compatible with biases shape [{biases.Length}] ({weights.GetLength(0)} != {biases.Length}");
            Weights = weights;
            Biases = biases;
        }

        public float[] Biases { get; private set; }
        public float[,] Weights { get; private set; }
    }
}