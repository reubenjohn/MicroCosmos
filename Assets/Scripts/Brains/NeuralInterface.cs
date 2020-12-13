using System.Linq;

namespace Brains
{
    public class NeuralInterface
    {
        private readonly float[][] actuatorLogits;
        private readonly float[] flattenedInput;
        private readonly float[] flattenedOutput;
        private readonly INeuralNetwork neuralNetwork;
        private readonly float[][] sensorLogits;

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
            Logits.Flatten(sensorLogits, flattenedInput, false);

            Logits.Clamp(flattenedInput);

            neuralNetwork.React(flattenedInput, flattenedOutput);

            Logits.Clamp(flattenedOutput);

            Logits.Unflatten(flattenedOutput, actuatorLogits);
        }
    }
}