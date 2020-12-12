using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode.Misc
{
    public static class SerializationTest
    {
        [Test]
        public static void TestVector2Serialization() =>
            Assert.AreEqual("0.1 0.9", Serialization.ToSerializable(new Vector2(.1f, .9f)));

        [Test]
        public static void TestVector2Deserialization() =>
            Assert.AreEqual(new Vector2(.4123f, .9f), Serialization.ToVector2("0.4123 0.9"));

        [Test]
        public static void TestFloat1DSerialization() =>
            Assert.AreEqual("[0.123,0.421]", new[] {.123f, .421f}.ToPrintable());

        [Test]
        public static void TestFloat1DSerializationWithRounding() =>
            Assert.AreEqual("[0.12,0.43]", new[] {.123f, .429f}.ToPrintable(2));

        [Test]
        public static void TestFloat1DOf1DSerializationWithRounding() =>
            Assert.AreEqual("[[0.12,0.43],[0.31]]", new[] {new[] {.123f, .429f}, new[] {.31f}}.ToPrintable(2));
    }
}