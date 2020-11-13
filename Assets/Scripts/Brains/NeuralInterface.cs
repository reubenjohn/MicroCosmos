using System.Linq;

namespace Brains
{
    public class NeuralInterface
    {
        private readonly float[][] sensorLogits;
        private readonly float[][] actuatorLogits;
        private readonly INeuralNetwork neuralNetwork;
        private readonly float[] flattenedInput;
        private readonly float[] flattenedOutput;

        public NeuralInterface(float[][] sensorLogits, float[][] actuatorLogits,
            INeuralNetwork neuralNetwork)
        {
            this.sensorLogits = sensorLogits;
            this.actuatorLogits = actuatorLogits;

            flattenedInput = new float[sensorLogits.Select(logits => logits.Length).Sum()];
            flattenedOutput = new float[actuatorLogits.Select(logits => logits.Length).Sum()];

            this.neuralNetwork = neuralNetwork;
        }

        public void React()
        {
            Logits.Flatten(sensorLogits, flattenedInput);

            neuralNetwork.React(flattenedInput, flattenedOutput);

            Logits.Unflatten(flattenedOutput, actuatorLogits);
        }
    }
}