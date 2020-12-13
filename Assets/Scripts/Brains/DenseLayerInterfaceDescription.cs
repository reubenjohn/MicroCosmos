namespace Brains
{
    public class DenseLayerInterfaceDescription
    {
        public DenseLayerInterfaceDescription(int inputLength, int outputLength)
        {
            InputLength = inputLength;
            OutputLength = outputLength;
        }

        public int InputLength { get; }
        public int OutputLength { get; }
    }
}