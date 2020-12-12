using System;
using System.Diagnostics.CodeAnalysis;
using Chemistry;
using NUnit.Framework;
using Tests.Common;
using static Tests.Common.TestSubstance;

namespace Tests.EditMode.Chemistry
{
    public class ReactionTest
    {
        [Test]
        public void TestToString()
        {
            Assert.AreEqual("{  } -> {  }",
                new Reaction<TestSubstance>(new Mixture<TestSubstance>(), new Mixture<TestSubstance>()).ToString());
            Assert.AreEqual("{ Grass: 1 } -> { Cow: 1 }", (Grass.M(1) > Cow.M(1)).ToString());
            Assert.AreEqual("{ Cow: 0.5, Grass: 0.5 } -> { Cow: 1 }", (Grass.M(.5) + Cow.M(.5) > Cow.M(1)).ToString());
            Assert.AreEqual("{ Cow: 1 } -> { Cow: 0.5, Dung: 0.5 }", (Cow.M(1) > Cow.M(.5) + Dung.M(.5)).ToString());
            Assert.AreEqual("{ Cow: 0.1, Grass: 0.9 } -> { Cow: 0.1, Dung: 0.9 }",
                (Grass.M(.9) + Cow.M(.1) + Dung.M(0) > Dung.M(.9) + Cow.M(.1)).ToString());
        }

        [Test]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public void TestImbalancedReaction()
        {
            Assert.Throws<ArgumentException>(() => (Grass.M(0) > Cow.M(.1)).ToString());
            Assert.Throws<ArgumentException>(() => (Grass.M(.5) > Cow.M(1)).ToString());
            Assert.Throws<ArgumentException>(() => (Grass.M(.99) + Cow.M(.1) > Dung.M(.9) + Cow.M(.1)).ToString());
        }

        [Test]
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
        public void TestNotImplementedMethods()
        {
            Assert.Throws<NotImplementedException>(() => (Grass.M(0) < Cow.M(.1)).ToString());
            Assert.Throws<NotImplementedException>(() => (Cow.M(0) + Grass.M(0) < Cow.M(.1)).ToString());
            Assert.Throws<NotImplementedException>(() => (Cow.M(0) < Cow.M(.1) + Grass.M(0)).ToString());
            Assert.Throws<NotImplementedException>(() => (Cow.M(0) + Grass.M(0) < Cow.M(.1) + Grass.M(0)).ToString());
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