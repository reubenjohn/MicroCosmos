namespace Brains.SimpleGeneticBrain1
{
    public class SimpleGeneticBrain1Description
    {
        public SimpleGeneticBrain1Description(int inputLength, int outputLength)
        {
            DenseLayer1 = new DenseLayerInterfaceDescription(inputLength, outputLength);
        }

        public DenseLayerInterfaceDescription DenseLayer1 { get; }
    }
}