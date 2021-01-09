namespace Brains
{
    public class DenseLayer
    {
        private readonly float[] biases;
        private readonly int maxInputs;
        private readonly int maxOutputs;
        private readonly float[,] weights;

        public DenseLayer(float[,] weights, float[] biases)
        {
            maxOutputs = weights.GetLength(0);
            maxInputs = weights.GetLength(1);
            this.weights = weights;
            this.biases = biases;
        }

        public void Calculate(float[] inputs, float[] outputs)
        {
            var inputLength = inputs.Length;
            var outputLength = outputs.Length;
            if (inputLength > maxInputs)
                throw new InputSizeMismatchException($"Input size {inputLength} exceeds limit of {maxInputs}");
            if (outputLength > maxOutputs)
                throw new InputSizeMismatchException($"Output size {outputLength} exceeds limit of  {maxOutputs}");
            for (var outI = 0; outI < outputLength; outI++)
            {
                outputs[outI] = biases[outI];
                for (var inI = 0; inI < inputLength; inI++)
                    outputs[outI] += inputs[inI] * weights[outI, inI];
            }
        }
    }
}