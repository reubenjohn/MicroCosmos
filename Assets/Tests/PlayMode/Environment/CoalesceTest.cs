using System.Collections;
using Chemistry;
using ChemistryMicro;
using Environment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayMode.Environment
{
    public class CoalesceTest
    {
        [OneTimeSetUp]
        public void SetupScene() => SceneManager.LoadScene("Tests/PlayMode/GeneralTestScene");

        [Test]
        public void DensityAssumption() => Assert.AreEqual(.01f, RecipeBook.Density);

        [UnityTest]
        public IEnumerator TestEmptyBlobInstantiation()
        {
            var flask = new Flask<Substance>(new MixtureDictionary<Substance> {{Substance.Fat, .1f}});
            var blob = ChemicalBlob.InstantiateBlob(flask, new Mixture<Substance>(), Vector3.left, null);

            Assert.AreEqual(.1f, flask.TotalMass);
            Assert.IsTrue(Mathf.Approximately(0f, blob.TotalMass));
            Assert.AreEqual(Vector3.left, blob.transform.position);
            Assert.AreEqual(Quaternion.identity, blob.transform.rotation);
            Assert.AreEqual(null, blob.transform.parent);

            Object.Destroy(blob.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestNonEmptyBlobInstantiation()
        {
            var flask = new Flask<Substance>(new MixtureDictionary<Substance> {{Substance.Fat, .1f}});
            var mix = new MixtureDictionary<Substance> {{Substance.Fat, .075f}}.ToMixture();
            var blob = ChemicalBlob.InstantiateBlob(flask, mix, Vector3.zero, null);

            Assert.IsTrue(Mathf.Approximately(.025f, flask.TotalMass));
            Assert.AreEqual(.075f, blob.TotalMass);

            blob.Transfer(flask, new Mixture<Substance>() - mix); //Empty blob to conserve mass

            Object.Destroy(blob.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestBlobCoalescesWithBlob()
        {
            var flask = new Flask<Substance>(new MixtureDictionary<Substance> {{Substance.Fat, .1f}});
            var mixA = new MixtureDictionary<Substance> {{Substance.Fat, .075f}}.ToMixture();
            var blobA = ChemicalBlob.InstantiateBlob(flask, mixA, Vector3.right, null);
            var blobB = ChemicalBlob.InstantiateBlob(flask, flask, Vector3.right * 1.1f, null);

            Assert.AreEqual(0, flask.TotalMass);
            Assert.AreEqual(.075f, blobA.TotalMass);
            Assert.IsTrue(Mathf.Approximately(.025f, blobB.TotalMass));

            yield return null;

            Assert.AreEqual(.1f, blobA.TotalMass);
            Assert.IsTrue(blobB == null);

            //Empty blob to conserve mass
            blobA.Transfer(flask, new MixtureDictionary<Substance> {{Substance.Fat, -.1f}}.ToMixture());

            Object.Destroy(blobA.gameObject);
            yield return null;
        }
    }
}