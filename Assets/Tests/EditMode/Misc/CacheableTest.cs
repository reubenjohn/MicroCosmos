using NUnit.Framework;

namespace Tests.EditMode.Misc
{
    public class CacheableTest
    {
        [Test]
        public void TestUninitializedCache()
        {
            var nFlowers = 1;
            // ReSharper disable once AccessToModifiedClosure
            var cache = new Cacheable<int>(() => nFlowers);
            Assert.AreEqual(1, cache.Value);

            nFlowers = 2;
            Assert.AreEqual(1, cache.Value);

            cache.Invalidate();
            Assert.AreEqual(2, cache.Value);
        }

        [Test]
        public void TestInitializedCache()
        {
            var nFlowers = 1;
            // ReSharper disable once AccessToModifiedClosure
            var cache = new Cacheable<int>(() => nFlowers, -1);
            Assert.AreEqual(-1, cache.Value);

            cache.Invalidate();
            Assert.AreEqual(1, cache.Value);

            nFlowers = 2;
            Assert.AreEqual(1, cache.Value);
            cache.Invalidate();
            Assert.AreEqual(2, cache.Value);
        }
    }
}