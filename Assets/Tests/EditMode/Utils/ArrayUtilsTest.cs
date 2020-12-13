using Newtonsoft.Json;
using NUnit.Framework;
using Util;

namespace Tests.EditMode.Utils
{
    public class ArrayUtilsTest
    {
        [Test]
        public void TestCopy2DComplete()
        {
            var source = new float[,] {{1, 2}, {3, 4}, {5, 6}};
            var destination = new float[3, 2];
            ArrayUtils.Copy(source, destination, 3, 2);
            Assert.AreEqual("[[1.0,2.0],[3.0,4.0],[5.0,6.0]]", JsonConvert.SerializeObject(destination));
        }

        [Test]
        public void TestCopy2DIncomplete()
        {
            var source = new float[,] {{1, 2}, {3, 4}, {5, 6}};
            var destination1 = new float[1, 2];
            ArrayUtils.Copy(source, destination1, 1, 2);
            Assert.AreEqual("[[1.0,2.0]]", JsonConvert.SerializeObject(destination1));

            var destination2 = new float[2, 1];
            ArrayUtils.Copy(source, destination2, 2, 1);
            Assert.AreEqual("[[1.0],[3.0]]", JsonConvert.SerializeObject(destination2));
        }
    }
}