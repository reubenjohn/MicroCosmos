using System.Collections;
using System.Collections.Generic;
using Chemistry;
using ChemistryMicro;
using Environment;
using NUnit.Framework;
using Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayMode.Environment
{
    public class CoalesceTest
    {
        [OneTimeSetUp]
        public void MySetup()
        {
            SceneManager.LoadScene("Tests/PlayMode/CellColonyTestScene");
        }

        public ChemicalSink ResetChemicalSink()
        {
            var microCosmosPersistence = GameObject.Find("Environment").GetComponent<MicroCosmosPersistence>();
            microCosmosPersistence.SaveDirectory = $"{Application.temporaryCachePath}/testing/{nameof(CoalesceTest)}";
            microCosmosPersistence.OnSave();
            EnvironmentInitializer.LoadMicroCosmosFromResources(new Dictionary<string, string>
            {
                {"Environment.ChemicalSink", "CoalesceTest-chemicalSink"}
            }, out _);
            var chemicalSink = GameObject.Find("Environment").GetComponent<ChemicalSink>();
            microCosmosPersistence.OnLoad();
            return chemicalSink;
        }

        [Test]
        public void DensityAssumption() => Assert.AreEqual(.1f, RecipeBook.Density);

        [UnityTest]
        public IEnumerator TestEmptyBlobInstantiation()
        {
            var chemicalSink = ResetChemicalSink();

            var blob = ChemicalBlob.InstantiateBlob(chemicalSink, new Mixture<Substance>(), Vector3.left, null);

            Assert.IsTrue(Mathf.Approximately(.1f, chemicalSink.TotalMass));
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
            var chemicalSink = ResetChemicalSink();

            var mix = new MixtureDictionary<Substance> {{Substance.Fat, .075f}}.ToMixture();
            var blob = ChemicalBlob.InstantiateBlob(chemicalSink, mix, Vector3.zero, null);

            Assert.IsTrue(Mathf.Approximately(.025f, chemicalSink.TotalMass));
            Assert.AreEqual(.075f, blob.TotalMass);

            chemicalSink.TransferTo(blob, new Mixture<Substance>() - mix); //Empty blob to conserve mass

            Object.Destroy(blob.gameObject);
            yield return null;
        }


        [UnityTest]
        public IEnumerator TestBlobCoalescesWithBlob()
        {
            var chemicalSink = ResetChemicalSink();

            var mixA = new MixtureDictionary<Substance> {{Substance.Fat, .075f}}.ToMixture();
            var blobA = ChemicalBlob.InstantiateBlob(chemicalSink, mixA, Vector3.right, null);
            var blobB = ChemicalBlob.InstantiateBlob(chemicalSink, chemicalSink.ToMixture(),
                Vector3.right * 1.01f, null);

            Assert.AreEqual(0, chemicalSink.TotalMass);
            Assert.AreEqual(.075f, blobA.TotalMass);
            Assert.IsTrue(Mathf.Approximately(.025f, blobB.TotalMass));

            yield return null;

            Assert.AreEqual(.1f, blobA.TotalMass);
            Assert.IsTrue(blobB == null);

            //Empty blob to conserve mass
            chemicalSink.TransferTo(blobA, new MixtureDictionary<Substance> {{Substance.Fat, -.1f}}.ToMixture());

            Object.Destroy(blobA.gameObject);
            yield return null;
        }
    }
}