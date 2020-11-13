namespace Brains
{
    public interface INeuralNetwork
    {
        void React(float[] receivedInputs, float[] receivedOutputs);
    }
}