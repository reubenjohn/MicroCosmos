using NUnit.Framework;
using Organelles.Orifice;

namespace Tests.EditMode.Organelles
{
    public class OrificeTest
    {
        [Test]
        public void TestNormalizedTransferRate()
        {
            Assert.AreEqual(0, OrificeGeneTranscriber.NormalizedTransferRate(-5f, -3f, 1e-5f));
            Assert.AreEqual(1, OrificeGeneTranscriber.NormalizedTransferRate(-5f, -3f, 1e-3f));
            Assert.AreEqual(.5, OrificeGeneTranscriber.NormalizedTransferRate(-5f, -3f, 1e-4f));

            Assert.AreEqual(0, OrificeGeneTranscriber.NormalizedTransferRate(3f, 5f, 1e3f));
            Assert.AreEqual(1, OrificeGeneTranscriber.NormalizedTransferRate(3f, 5f, 1e5f));
            Assert.AreEqual(.5, OrificeGeneTranscriber.NormalizedTransferRate(3f, 5f, 1e4f));

            Assert.AreEqual(0, OrificeGeneTranscriber.NormalizedTransferRate(-3f, 3f, 1e-3f));
            Assert.AreEqual(1, OrificeGeneTranscriber.NormalizedTransferRate(-3f, 3f, 1e3f));
            Assert.AreEqual(.5, OrificeGeneTranscriber.NormalizedTransferRate(-3f, 3f, 1e0f));
        }
    }
}