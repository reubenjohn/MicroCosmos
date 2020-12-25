using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;
using Util;

namespace Tests.EditMode.Utils
{
    public class RandomUtilsTest
    {
        [Test]
        public void TestRandomLogits1D()
        {
            Random.InitState(1);
            var logits = RandomUtils.RandomLogits(3);
            Assert.AreEqual("[-0.9993694,-0.5485256,-0.361967683]", JsonConvert.SerializeObject(logits));
        }

        [Test]
        public void TestRandomizeFullLogits1D()
        {
            Random.InitState(1);
            var logits2 = new float[3];
            RandomUtils.RandomizeLogits(logits2, 3);
            Assert.AreEqual("[-0.9993694,-0.5485256,-0.361967683]", JsonConvert.SerializeObject(logits2));
        }

        [Test]
        public void TestRandomizePartialLogits1D()
        {
            Random.InitState(1);
            var logits2 = new float[3];
            RandomUtils.RandomizeLogits(logits2, 3, 1);
            Assert.AreEqual("[0.0,-0.9993694,-0.5485256]", JsonConvert.SerializeObject(logits2));
        }

        [Test]
        public void TestRandomLogits2D()
        {
            Random.InitState(1);
            var logits = RandomUtils.RandomLogits(3, 2);
            Assert.AreEqual(
                "[[-0.9993694,-0.5485256],[-0.361967683,0.079087615],[-0.188854814,-0.5695789]]",
                JsonConvert.SerializeObject(logits));
        }

        [Test]
        public void TestRandomizeFullLogits2D()
        {
            Random.InitState(1);
            var logits = new float[3, 2];
            RandomUtils.RandomizeLogits(logits, 3, 2);
            Assert.AreEqual(
                "[[-0.9993694,-0.5485256],[-0.361967683,0.079087615],[-0.188854814,-0.5695789]]",
                JsonConvert.SerializeObject(logits));
        }

        [Test]
        public void TestRandomizePartialLogits2D()
        {
            Random.InitState(1);
            var logits = new float[3, 2];
            RandomUtils.RandomizeLogits(logits, 2, 2);
            Assert.AreEqual(
                "[[-0.9993694,-0.5485256],[-0.361967683,0.079087615],[0.0,0.0]]",
                JsonConvert.SerializeObject(logits));
        }
    }
}