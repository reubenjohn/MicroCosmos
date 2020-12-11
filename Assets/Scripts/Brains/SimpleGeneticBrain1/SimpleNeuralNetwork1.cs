namespace Brains.SimpleGeneticBrain1
{
    public class SimpleNeuralNetwork1 : INeuralNetwork
    {
        private readonly FullyConnectedLayer dense;

        public SimpleNeuralNetwork1(SimpleGeneticBrain1Gene gene)
        {
            dense = new FullyConnectedLayer(gene.weights, gene.biases);
        }

        public void React(float[] receivedInputs, float[] receivedOutputs)
        {
            dense.Calculate(receivedInputs, receivedOutputs);
        }
    }
}