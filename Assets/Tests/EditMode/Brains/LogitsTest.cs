using System;
using System.Collections;
using Brains;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests.EditMode.Brains
{
    public class LogitsTest
    {
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


        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}