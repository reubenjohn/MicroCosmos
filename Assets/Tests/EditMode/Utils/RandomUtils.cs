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
        public void TestRandomLogits2D()
        {
            Random.InitState(1);
            var logits = RandomUtils.RandomLogits(3, 2);
            Assert.AreEqual(
                "[[-0.9993694,-0.5485256],[-0.361967683,0.079087615],[-0.188854814,-0.5695789]]",
                JsonConvert.SerializeObject(logits));
        }
    }
}