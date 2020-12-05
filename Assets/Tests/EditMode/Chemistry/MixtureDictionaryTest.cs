using System;
using System.Collections.Generic;
using Chemistry;
using NUnit.Framework;
using static Tests.EditMode.Chemistry.TestSubstance;

namespace Tests.EditMode.Chemistry
{
    public class MixtureDictionaryTest
    {
        [Test]
        public void TestToString()
        {
            Assert.AreEqual("{  }", new Mixture<TestSubstance>().ToString());
            Assert.AreEqual("{ Cow: 1.2, Grass: 0.5 }",
                new MixtureDictionary<TestSubstance>() {{Grass, .5f}, {Cow, 1.2f}}.ToMixture().ToString());
            Assert.AreEqual("{ Grass: 0.3333333 }",
                new MixtureDictionary<TestSubstance>() {{Grass, 1f / 3}, {Cow, 0f}}.ToMixture().ToString());
        }

        [Test]
        public void TestEquals()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<NotImplementedException>(() => new MixtureDictionary<TestSubstance>().Equals(null));
        }

        [Test]
        public void TestHashCode()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<NotImplementedException>(() => new MixtureDictionary<TestSubstance>().GetHashCode());
        }
    }
}