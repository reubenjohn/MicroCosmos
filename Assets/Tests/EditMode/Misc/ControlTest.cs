using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode.Misc
{
    public class ControlTest
    {
        [Test]
        public void TestBinaryControlStep()
        {
            Assert.AreEqual(0f,
                Control.BinaryControlStep(0f, false, false, 1f, 1f));
            Assert.AreEqual(0f,
                Control.BinaryControlStep(0f, true, true, 1f, 1f));

            Assert.IsTrue(Mathf.Approximately(.9f,
                    Control.BinaryControlStep(.1f, true, false, 4f, .2f)),
                "Positive input increases by step limit");
            Assert.IsTrue(Mathf.Approximately(-.7f,
                    Control.BinaryControlStep(.1f, false, true, 4f, .2f)),
                "Negative input increases by step limit");


            Assert.IsTrue(Mathf.Approximately(1f,
                    Control.BinaryControlStep(1f, true, false, 4f, .2f)),
                "Does not cross upper limit");
            Assert.IsTrue(Mathf.Approximately(-1f,
                    Control.BinaryControlStep(-1f, false, true, 4f, .2f)),
                "Does not cross lower limit");
        }

        [Test]
        public void TestBinaryControlVariable()
        {
            var controlVar = new Control.BinaryControlVariable(4f) {Value = .1f};
            Assert.AreEqual(Control.BinaryControlStep(.1f, true, false, 4f, .2f),
                controlVar.FeedInput(true, false, .2f),
                "BinaryControlVariable performs an exact BinaryControlStep");
        }
    }
}