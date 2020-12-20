namespace Brains.SimpleGeneticBrain1
{
    public class SimpleNeuralNetwork1 : INeuralNetwork
    {
        private readonly DenseLayer dense;

        public SimpleNeuralNetwork1(SimpleGeneticBrain1Gene gene) // TODO Validate against LivingDescription
        {
            dense = new DenseLayer(gene.denseLayer1.Weights, gene.denseLayer1.Biases);
        }

        public void React(float[] receivedInputs, float[] receivedOutputs) =>
            dense.Calculate(receivedInputs, receivedOutputs);
    }
}