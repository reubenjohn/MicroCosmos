using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using Util;

namespace Tests.EditMode.Utils
{
    internal enum TestEnum
    {
        A,
        B
    }

    public class EnumUtilsTest
    {
        [Test]
        public void TestParseNamedDictionary()
        {
            var parsed = EnumUtils.ParseNamedDictionary<TestEnum, int>(new Dictionary<string, int> {{"A", 1}});
            Assert.AreEqual(new[] {new KeyValuePair<TestEnum, int>(TestEnum.A, 1)}, parsed.ToArray());

            var ex = Assert.Throws<InvalidOperationException>(() =>
                EnumUtils.ParseNamedDictionary<TestEnum, int>(new Dictionary<string, int> {{"A", 1}, {"C", 1}}));
            StringAssert.Contains("Could not parse 'C'", ex.Message);
        }

        [Test]
        public void ToNamedDictionary()
        {
            var enumDict = EnumUtils.ToNamedDictionary(new Dictionary<TestEnum, int>
                {{TestEnum.A, 1}, {TestEnum.B, 2}});
            Assert.AreEqual(@"{""A"":1,""B"":2}", JsonConvert.SerializeObject(enumDict));
        }

        [Test]
        public void TestEnumCount()
        {
            Assert.AreEqual(2, EnumUtils.EnumCount(typeof(TestEnum)));
        }
    }
}