using System;
using Brains;
using NUnit.Framework;

namespace Tests.EditMode.Brains
{
    public class LogitsTest
    {
        [Test]
        public void TestClamp()
        {
            var logits = new[] {12f, -6f, 1f, -1f, .1f, -.1f};
            Logits.Clamp(logits);
            Assert.AreEqual(new[] {1f, -1f, 1f, -1f, .1f, -.1f}.ToPrintable(), logits.ToPrintable());
        }

        [Test]
        public void TestFlatten()
        {
            var flattenedOutput = new float[6];
            Logits.Flatten(
                new[] {new[] {.1f, .2f, .3f}, new[] {.4f, .5f, .6f}},
                flattenedOutput
            );
            Assert.AreEqual(new[] {.1f, .2f, .3f, .4f, .5f, .6f}.ToPrintable(), flattenedOutput.ToPrintable());
        }

        [Test]
        public void TestFlattenWithDestinationTooShort()
        {
            var flattenedOutput = new float[3];
            var ex = Assert.Throws<ArgumentException>(() => Logits.Flatten(
                new[] {new[] {.1f, .2f}, new[] {.4f, .5f}},
                flattenedOutput
            ));
            StringAssert.Contains("Destination array was not long enough", ex.Message);
        }

        [Test]
        public void TestFlattenWithDestinationTooLong()
        {
            var flattenedOutput = new float[3];
            var ex = Assert.Throws<ArgumentException>(() => Logits.Flatten(
                new[] {new[] {.1f}, new[] {.4f}},
                flattenedOutput
            ));
            StringAssert.Contains("Destination array was too long", ex.Message);
        }
    }
}