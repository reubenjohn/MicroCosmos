using System;
using Brains;
using NUnit.Framework;
using Util;

namespace Tests.EditMode.Brains
{
    public class FullyConnectedLayerTest
    {
        [Test]
        public void TestCalculation()
        {
            var dense = new DenseLayer(
                new[,] {{.8f, -.8f, .1f}, {.1f, .2f, .3f}},
                new[] {-.4f, .2f}
            );
            var outputs = new float[2];

            dense.Calculate(new[] {.4f, .2f, -.3f}, outputs);
            Assert.AreEqual(new[] {-.27f, .19f}.ToPrintable(4), outputs.ToPrintable(4));

            dense.Calculate(new[] {.4f, .2f}, outputs);
            Assert.AreEqual(new[] {-0.24f, 0.28f}.ToPrintable(4), outputs.ToPrintable(4));
        }

        [Test]
        public void TestIncompatibleSizes()
        {
            var dense = new DenseLayer(
                new[,] {{.8f, -.8f, .1f}, {.1f, .2f, .3f}},
                new[] {-.4f, .2f}
            );
            Assert.Throws<ArgumentException>(() => dense.Calculate(new[] {.4f, .2f, .1f, .1f}, new float[2]));
            Assert.Throws<ArgumentException>(() => dense.Calculate(new[] {.4f, .2f, -.3f}, new float[3]));
        }
    }
}