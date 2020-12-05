using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Chemistry;
using NUnit.Framework;
using static Tests.EditMode.Chemistry.TestSubstance;

namespace Tests.EditMode.Chemistry
{
    public class FlaskTest
    {
        [Test]
        public void TestToString()
        {
            Assert.AreEqual("{  }", new Flask<TestSubstance>().ToString());
            Assert.AreEqual("{ Cow: 1.2, Grass: 0.5 }",
                new MixtureDictionary<TestSubstance>() {{Grass, .5f}, {Cow, 1.2f}}.ToFlask().ToString());
            Assert.AreEqual("{ Grass: 0.3333333 }",
                new MixtureDictionary<TestSubstance>() {{Grass, 1f / 3}, {Cow, 0f}}.ToFlask().ToString());
        }

        [Test]
        public void TestEquality()
        {
            Assert.AreEqual(new Flask<TestSubstance>(), new Flask<TestSubstance>());

            var m1 = new MixtureDictionary<TestSubstance>() {{Grass, .5f}, {Cow, 1.2f}}.ToFlask();
            var m2 = new MixtureDictionary<TestSubstance>() {{Cow, 1.2f}, {Grass, .5f}}.ToFlask();
            var m3 = new MixtureDictionary<TestSubstance>() {{Grass, .5f}, {Cow, 1.2f}, {Dung, 0f}}.ToFlask();

            Assert.AreEqual(new Flask<TestSubstance>(), new Flask<TestSubstance>());
            // ReSharper disable once EqualExpressionComparison
            Assert.IsTrue(m1.Equals(m1));
            Assert.AreEqual(m1, m2);
            Assert.AreEqual(m1, m3);
            Assert.AreEqual(m2, m3);

            Assert.IsFalse(new Flask<TestSubstance>().Equals(null));
            Assert.AreNotEqual(m1, new Flask<TestSubstance>());

            var m4 = new MixtureDictionary<TestSubstance> {{Cow, .6f}, {Grass, 1.2f}}.ToFlask();
            Assert.AreNotEqual(m1, m4);
            Assert.AreNotEqual(m3, m4);

            var m5 = new MixtureDictionary<TestSubstance> {{Dung, .6f}}.ToFlask();
            Assert.AreNotEqual(m1, m5);

            Assert.AreNotEqual(new Flask<TestSubstance>(), new MixtureDictionary<TestSubstance>());
        }

        [Test]
        public void TestToFlask()
        {
            var f1 = new Flask<TestSubstance>();
            var d1 = new MixtureDictionary<TestSubstance>().ToFlask();
            var f2 = new Flask<TestSubstance>(new Dictionary<TestSubstance, float>() {{Grass, 1.2f}});
            var d2 = new MixtureDictionary<TestSubstance>() {{Grass, 1.2f}}.ToFlask();
            var f3 = new Flask<TestSubstance>(new Dictionary<TestSubstance, float>() {{Grass, 1.2f}, {Cow, 0}});
            var d3 = new MixtureDictionary<TestSubstance>() {{Grass, 1.2f}, {Cow, 0}}.ToFlask();

            Assert.AreEqual(f1, d1);
            Assert.AreEqual(f2, d2);
            Assert.AreEqual(f3, d2);
            Assert.AreEqual(f2, d3);
            Assert.AreEqual(f3, d3);
        }

        [Test]
        public void TestTransfer()
        {
            var m0 = new MixtureDictionary<TestSubstance>().ToMixture();
            var m1 = new MixtureDictionary<TestSubstance> {{Grass, 1.2f}}.ToMixture();
            var m2 = new MixtureDictionary<TestSubstance> {{Grass, .1f}}.ToMixture();
            var m3 = new MixtureDictionary<TestSubstance> {{Cow, .5f}}.ToMixture();

            var f1 = m1.ToFlask();
            var f2 = m2.ToFlask();
            var f3 = m3.ToFlask();

            Assert.IsTrue(Flask<TestSubstance>.Transfer(f1, m0.ToFlask(), m0));
            Assert.AreEqual(m1.ToFlask(), f1);

            Assert.IsTrue(Flask<TestSubstance>.Transfer(f1, f2, m0));
            Assert.AreEqual(m1.ToFlask(), f1);
            Assert.AreEqual(m2.ToFlask(), f2);

            Assert.IsTrue(Flask<TestSubstance>.Transfer(f1, f2, m2));
            Assert.AreEqual((m1 + m2).ToFlask(), f1);
            Assert.AreEqual(m0.ToFlask(), f2);

            Assert.IsTrue(Flask<TestSubstance>.Transfer(f2, f1, m2));
            Assert.AreEqual(m1.ToFlask(), f1);
            Assert.AreEqual(m2.ToFlask(), f2);

            Assert.IsTrue(Flask<TestSubstance>.Transfer(f3, m3.ToFlask(), m3));
            Assert.IsTrue(Flask<TestSubstance>.Transfer(f1, f2, m2));
            Assert.IsTrue(Flask<TestSubstance>.Transfer(f1, f3, m3));
            Assert.AreEqual((m1 + m2 + m3).ToFlask(), f1);
            Assert.AreEqual(m0.ToFlask(), f2);
            Assert.AreEqual(m3.ToFlask(), f3);

            Assert.IsTrue(Flask<TestSubstance>.Transfer(f1, f1, f1));
            Assert.AreEqual((m1 + m2 + m3).ToFlask(), f1);

            // Insufficiency

            Assert.IsFalse(Flask<TestSubstance>.Transfer(f3, m0.ToFlask(), m3));
            Assert.AreEqual(m3.ToFlask(), f3);

            Assert.IsFalse(Flask<TestSubstance>.Transfer(f2, f3, m3 * 2));
            Assert.AreEqual(m3.ToFlask(), f3);
            Assert.AreEqual(m0.ToFlask(), f2);
        }

        [Test]
        [SuppressMessage("ReSharper", "EqualExpressionComparison")]
        public void TestConvert()
        {
            var m1 = new MixtureDictionary<TestSubstance> {{Grass, 1.2f}}.ToMixture();
            var m3 = new MixtureDictionary<TestSubstance> {{Cow, .5f}}.ToMixture();

            var flask = m1.ToFlask();
            var reaction1 = Grass.M(1) > Grass.M(1);

            Assert.AreEqual(1.2f, flask.Convert(reaction1));
            Assert.AreEqual(m1.ToFlask(), flask);

            Assert.AreEqual(0f, flask.Convert(reaction1, 0));
            Assert.AreEqual(m1.ToFlask(), flask);

            var message = Assert.Throws<InvalidOperationException>(() => flask.Convert(Grass.M(1) > Grass.M(1), -.1f))
                .Message;
            Assert.AreEqual(m1.ToFlask(), flask);
            StringAssert.Contains("must lie in range [0, 1]", message);

            var reaction2 = Grass.M(1) > Cow.M(1);
            flask = (m1 + m3).ToFlask();

            Assert.AreEqual(.12f, flask.Convert(reaction2, .1f), 1e-6);
            Assert.AreEqual(new MixtureDictionary<TestSubstance>() {{Grass, 1.08f}, {Cow, .62f}}.ToFlask(), flask);

            Assert.AreEqual(1.08f, flask.Convert(reaction2));
            Assert.AreEqual(new MixtureDictionary<TestSubstance>() {{Cow, 1.7f}}.ToFlask(), flask);

            var reaction3 = Grass.M(1) > Cow.M(.1) + Dung.M(.9);
            flask = (m1 + m3).ToFlask();

            Assert.AreEqual(.12f, flask.Convert(reaction3, .1f), 1e-6);
            Assert.AreEqual(new MixtureDictionary<TestSubstance>() {{Grass, 1.08f}, {Cow, .512f}, {Dung, .108f}}
                .ToFlask(), flask);

            Assert.AreEqual(1.08f, flask.Convert(reaction3));
            Assert.AreEqual(new MixtureDictionary<TestSubstance>() {{Cow, .62f}, {Dung, 1.08f}}.ToFlask(), flask);

            //Limiting reagent
            var reaction4 = Grass.M(.5) + Cow.M(.5) > Dung.M(.45) + Cow.M(.55);

            flask = new MixtureDictionary<TestSubstance>() {{Grass, 1f}, {Cow, 10f}}.ToFlask();
            Assert.AreEqual(2f, flask.Convert(reaction4));
            Assert.AreEqual(new MixtureDictionary<TestSubstance>() {{Cow, 10.1f}, {Dung, .9f}}.ToFlask(), flask);

            flask = new MixtureDictionary<TestSubstance>() {{Grass, 10f}, {Cow, 1f}}.ToFlask();
            Assert.AreEqual(2f, flask.Convert(reaction4));
            Assert.AreEqual(new MixtureDictionary<TestSubstance>() {{Grass, 9f}, {Cow, 1.1f}, {Dung, .9f}}.ToFlask(),
                flask);
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