using System;
using System.Collections.Generic;
using Chemistry;
using NUnit.Framework;
using static Tests.EditMode.Chemistry.TestSubstance;

namespace Tests.EditMode.Chemistry
{
    public enum TestSubstance
    {
        Cow,
        Grass,
        Dung
    }

    public class MixtureTest
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
        public void TestToMixture()
        {
            var f1 = new Mixture<TestSubstance>();
            var d1 = new MixtureDictionary<TestSubstance>().ToMixture();
            var f2 = new Mixture<TestSubstance>(new Dictionary<TestSubstance, float>() {{Grass, 1.2f}});
            var d2 = new MixtureDictionary<TestSubstance>() {{Grass, 1.2f}}.ToMixture();
            var f3 = new Mixture<TestSubstance>(new Dictionary<TestSubstance, float>() {{Grass, 1.2f}, {Cow, 0}});
            var d3 = new MixtureDictionary<TestSubstance>() {{Grass, 1.2f}, {Cow, 0}}.ToMixture();

            Assert.AreEqual(f1, d1);
            Assert.AreEqual(f2, d2);
            Assert.AreEqual(f3, d2);
            Assert.AreEqual(f2, d3);
            Assert.AreEqual(f3, d3);
        }

        [Test]
        public void TestEquality()
        {
            Assert.AreEqual(new Mixture<TestSubstance>(), new Mixture<TestSubstance>());

            var m1 = new MixtureDictionary<TestSubstance>() {{Grass, .5f}, {Cow, 1.2f}}.ToMixture();
            var m2 = new MixtureDictionary<TestSubstance>() {{Cow, 1.2f}, {Grass, .5f}}.ToMixture();
            var m3 = new MixtureDictionary<TestSubstance>() {{Grass, .5f}, {Cow, 1.2f}, {Dung, 0f}}.ToMixture();

            Assert.AreEqual(new Mixture<TestSubstance>(), new Mixture<TestSubstance>());
            // ReSharper disable once EqualExpressionComparison
            Assert.IsTrue(m1.Equals(m1));
            Assert.AreEqual(m1, m2);
            Assert.AreEqual(m1, m3);
            Assert.AreEqual(m2, m3);

            Assert.IsFalse(new Mixture<TestSubstance>().Equals(null));
            Assert.AreNotEqual(m1, new Mixture<TestSubstance>());

            var m4 = new MixtureDictionary<TestSubstance> {{Cow, .6f}, {Grass, 1.2f}}.ToMixture();
            Assert.AreNotEqual(m1, m4);
            Assert.AreNotEqual(m3, m4);

            var m5 = new MixtureDictionary<TestSubstance> {{Dung, .6f}}.ToMixture();
            Assert.AreNotEqual(m1, m5);

            Assert.AreNotEqual(new Mixture<TestSubstance>(), new MixtureDictionary<TestSubstance>());
        }

        [Test]
        public void TestHashCode()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<NotImplementedException>(() => new Mixture<TestSubstance>().GetHashCode());
        }

        [Test]
        public void TestCopy()
        {
            var d0 = new Mixture<TestSubstance>();
            var d1 = new MixtureDictionary<TestSubstance>() {{Grass, 1.2f}}.ToMixture();
            var d2 = new MixtureDictionary<TestSubstance>() {{Grass, 1.2f}, {Cow, .5f}}.ToMixture();
            var d3 = new MixtureDictionary<TestSubstance>() {{Grass, 1.2f}, {Cow, 0}}.ToMixture();

            var d0Copy = d0.Copy();
            Assert.AreEqual(d0, d0Copy);
            Assert.AreNotSame(d0, d0Copy);

            var d1Copy = d1.Copy();
            Assert.AreEqual(d1, d1Copy);
            Assert.AreNotSame(d1, d1Copy);

            var d2Copy = d2.Copy();
            Assert.AreEqual(d2, d2Copy);
            Assert.AreNotSame(d2, d2Copy);

            var d3Copy = d3.Copy();
            Assert.AreEqual(d3, d3Copy);
            Assert.AreNotSame(d3, d3Copy);
        }

        [Test]
        public void TestMass()
        {
            Assert.AreEqual(0, new Mixture<TestSubstance>().Mass());
            Assert.AreEqual(1.2f, new MixtureDictionary<TestSubstance> {{Grass, 1.2f}}.ToMixture().Mass());
            Assert.AreEqual(1.7f, new MixtureDictionary<TestSubstance> {{Grass, 1.2f}, {Cow, .5f}}.ToMixture().Mass());
            Assert.AreEqual(1.2f, new MixtureDictionary<TestSubstance> {{Grass, 1.2f}, {Cow, 0}}.ToMixture().Mass());

            Assert.AreEqual(1.2f, new MixtureDictionary<TestSubstance> {{Grass, 1.2f}, {Cow, 0}}.ToMixture().TotalMass);
        }

        [Test]
        public void TestAddition()
        {
            var z0 = new Mixture<TestSubstance>();
            var m1 = new MixtureDictionary<TestSubstance> {{Grass, 1.2f}}.ToMixture();
            var m2 = new MixtureDictionary<TestSubstance> {{Grass, .1f}}.ToMixture();
            var m3 = new MixtureDictionary<TestSubstance> {{Cow, .5f}}.ToMixture();

            Assert.AreEqual(m1, m1 + z0);
            Assert.AreEqual(m1, z0 + m1);

            Assert.AreEqual(new MixtureDictionary<TestSubstance>() {{Grass, 1.3f}}.ToMixture(), m1 + m2);
            Assert.AreEqual(new MixtureDictionary<TestSubstance>() {{Grass, 1.2f}, {Cow, .5f}}.ToMixture(), m1 + m3);
            Assert.AreEqual(new MixtureDictionary<TestSubstance>() {{Grass, 1.2f}, {Cow, .5f}}.ToMixture(),
                m1 + m3 + z0);
        }

        [Test]
        public void TestSubtraction()
        {
            var z0 = new Mixture<TestSubstance>();
            var m1 = new MixtureDictionary<TestSubstance> {{Grass, 1.2f}}.ToMixture();
            var m2 = new MixtureDictionary<TestSubstance> {{Grass, .1f}}.ToMixture();
            var m3 = new MixtureDictionary<TestSubstance> {{Cow, .5f}}.ToMixture();

            Assert.AreEqual(m1, m1 - z0);
            Assert.AreEqual(new MixtureDictionary<TestSubstance> {{Grass, -1.2f}}.ToMixture(), z0 - m1);

            Assert.AreEqual(new MixtureDictionary<TestSubstance>() {{Grass, 1.3f}}.ToMixture(), m1 + m2);
            Assert.AreEqual(new MixtureDictionary<TestSubstance>() {{Grass, 1.2f}, {Cow, .5f}}.ToMixture(), m1 + m3);
            Assert.AreEqual(new MixtureDictionary<TestSubstance>() {{Grass, 1.2f}, {Cow, .5f}}.ToMixture(),
                m1 + m3 + z0);
        }

        [Test]
        public void TestMultiplication()
        {
            var z0 = new Mixture<TestSubstance>();
            var m1 = new MixtureDictionary<TestSubstance> {{Grass, 1.2f}}.ToMixture();
            var m2 = new MixtureDictionary<TestSubstance> {{Grass, .1f}, {Cow, .5f}}.ToMixture();

            Assert.AreEqual(z0, m1 * 0);
            Assert.AreEqual(new MixtureDictionary<TestSubstance> {{Grass, -1.2f}}.ToMixture(), m1 * -1);

            Assert.AreEqual(new MixtureDictionary<TestSubstance>() {{Grass, .2f}, {Cow, 1f}}.ToMixture(), m2 * 2);
            Assert.AreEqual(m2, (m2 * 3) * .3333333f);
        }
    }
}