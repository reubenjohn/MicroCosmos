namespace Brains
{
    public class SimpleNeuralNetwork1 : INeuralNetwork
    {
        private readonly FullyConnectedLayer dense;

        public SimpleNeuralNetwork1(GeneticBrainGene gene)
        {
            dense = new FullyConnectedLayer(gene.weights, gene.biases);
        }

        public void React(float[] receivedInputs, float[] receivedOutputs)
        {
            dense.Calculate(receivedInputs, receivedOutputs);
        }
    }
}