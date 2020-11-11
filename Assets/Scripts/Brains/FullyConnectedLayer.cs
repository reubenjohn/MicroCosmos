using System;

namespace Brains
{
    public class FullyConnectedLayer
    {
        private int nInputs;
        private int nOutputs;
        private float[,] weights;
        private float[] biases;

        public FullyConnectedLayer(float[,] weights, float[] biases)
        {
            nOutputs = weights.GetLength(0);
            nInputs = weights.GetLength(1);
            this.weights = weights;
            this.biases = biases;
        }

        public void Calculate(float[] inputs, float[] outputs)
        {
            if (inputs.Length != nInputs) throw new ArgumentException($"Input size must be: {inputs.Length}");
            if (outputs.Length != nOutputs) throw new ArgumentException($"Output size must be: {outputs.Length}");
            for (var outI = 0; outI < nOutputs; outI++)
            {
                outputs[outI] = biases[outI];
                for (var inI = 0; inI < nInputs; inI++)
                    outputs[outI] += inputs[inI] * weights[outI, inI];
            }
        }
    }
}