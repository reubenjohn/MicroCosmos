using Brains;
using Brains.SimpleGeneticBrain1;
using NUnit.Framework;
using Util;

namespace Tests.EditMode.Brains
{
    public static class SimpleNeuralNetwork1Test
    {
        [Test]
        public static void TestNeuralInterface()
        {
            var nn = new SimpleNeuralNetwork1(new SimpleGeneticBrain1Gene((gene, description) => gene)
            {
                denseLayer1 = new DenseLayerGene(new[,] {{.1f, .2f}}, new[] {.1f})
            });
            var inputs = new[] {.5f, -.8f};
            var outputs = new[] {.5f};
            nn.React(inputs, outputs);

            Assert.AreEqual(new[] {.5f, -.8f}.ToPrintable(2), inputs.ToPrintable(2),
                "Inputs are not mutated");
            Assert.AreEqual(new[] {-.01f}.ToPrintable(2), outputs.ToPrintable(2));
        }
    }
}