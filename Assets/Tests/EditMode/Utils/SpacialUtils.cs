using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode.Utils
{
    public class SpacialUtils
    {
        [Test]
        public void SmallOrientationAreUnaffected()
        {
            Assert.AreEqual(0, Util.SpacialUtils.NormalizeOrientation(0));
            Assert.AreEqual(1, Util.SpacialUtils.NormalizeOrientation(1));
            Assert.AreEqual(-1, Util.SpacialUtils.NormalizeOrientation(-1));
            Assert.AreEqual(-1, Util.SpacialUtils.NormalizeOrientation(-1));
            Assert.AreEqual(Mathf.PI, Util.SpacialUtils.NormalizeOrientation(Mathf.PI));
            Assert.AreEqual(-Mathf.PI, Util.SpacialUtils.NormalizeOrientation(-Mathf.PI));
        }

        [Test]
        public void OrientationBetweenPIAnd2PIAreNormalized()
        {
            Assert.AreEqual(-Mathf.PI + 1, Util.SpacialUtils.NormalizeOrientation(Mathf.PI + 1f), 0.001);
            Assert.AreEqual(Mathf.PI - 1, Util.SpacialUtils.NormalizeOrientation(-Mathf.PI - 1f), 0.001);
        }

        [Test]
        public void LargeOrientationsAreModded()
        {
            Assert.AreEqual(1, Util.SpacialUtils.NormalizeOrientation(2 * Mathf.PI + 1f), 0.001);
            Assert.AreEqual(-1, Util.SpacialUtils.NormalizeOrientation(-2 * Mathf.PI - 1f), 0.001);
        }

        [Test]
        public void SomeLargeOrientationsAreModdedAndNormalized()
        {
            Assert.AreEqual(-Mathf.PI + 1, Util.SpacialUtils.NormalizeOrientation(3 * Mathf.PI + 1f), 0.001);
            Assert.AreEqual(Mathf.PI - 1, Util.SpacialUtils.NormalizeOrientation(-3 * Mathf.PI - 1f), 0.001);
        }
    }
}