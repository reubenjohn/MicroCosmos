using Brains;
using NUnit.Framework;
using Util;

namespace Tests.EditMode.Brains
{
    internal class IntrospectiveNeuralNetwork : INeuralNetwork
    {
        public float[] inputs;
        public float[] outputs;
        public float[] targetOutputs;

        public void React(float[] receivedInputs, float[] receivedOutputs)
        {
            inputs = receivedInputs;
            for (var i = 0; i < targetOutputs.Length; i++)
                receivedOutputs[i] = targetOutputs[i];
            outputs = receivedOutputs;
        }
    }

    public static class NeuralInterfaceTest
    {
        [Test]
        public static void TestNeuralInterface()
        {
            var nn = new IntrospectiveNeuralNetwork {targetOutputs = new[] {.5f, -.5f}};
            var sensorLogits = new[] {new[] {.4f, .2f}, new[] {-.3f}};
            var actuatorLogits = new[] {new[] {.1f}, new[] {-.2f}};
            var neuralInterface = new NeuralInterface(sensorLogits, actuatorLogits, nn);

            neuralInterface.React();

            Assert.AreEqual(new[] {new[] {.4f, .2f}, new[] {-.3f}}.ToPrintable(2),
                sensorLogits.ToPrintable(2), "Sensor logits are not mutated");

            Assert.AreEqual(new[] {.4f, .2f, -.3f}.ToPrintable(2), nn.inputs.ToPrintable(2),
                "Sensor inputs are received flat");

            Assert.AreEqual(2, nn.outputs.Length,
                "Allocated flat output size is compatible with actuator logits sizes");

            Assert.AreEqual(new[] {.5f, -.5f}.ToPrintable(2), nn.outputs.ToPrintable(2),
                $"Sanity check to ensure {nameof(IntrospectiveNeuralNetwork)} is operating as expected");

            Assert.AreEqual(new[] {new[] {.5f}, new[] {-.5f}}.ToPrintable(2),
                actuatorLogits.ToPrintable(2),
                "Correct flat outputs are received");
        }


        [Test]
        public static void TestClamping()
        {
            var sensorLogits = new[] {new[] {1f, -100f}, new[] {100f}};
            var actuatorLogits = new[] {new[] {.1f}, new[] {-.2f}};
            var nn = new IntrospectiveNeuralNetwork {targetOutputs = new[] {100f, -100f}};

            new NeuralInterface(sensorLogits, actuatorLogits, nn).React();

            Assert.AreEqual(new[] {1f, -1f, 1f}.ToPrintable(1), nn.inputs.ToPrintable(1),
                "Inputs are clamped");
            Assert.AreEqual(new[] {new[] {1f}, new[] {-1f}}.ToPrintable(1), actuatorLogits.ToPrintable(1),
                "Actuator logits are clamped");
        }
    }
}