using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Environment;
using NUnit.Framework;
using Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Util;

namespace Tests.PlayMode.Persistence
{
    public class MicroCosmosPersistenceTest
    {
        private string saveDir;

        [OneTimeSetUp]
        public void GeneNodeTestSimplePasses() => SceneManager.LoadScene("Tests/PlayMode/CellColonyTestScene");

        [SetUp]
        public void Setup()
        {
            saveDir = $"{Application.temporaryCachePath}/testing/{nameof(MicroCosmosPersistenceTest)}";
            try
            {
                Directory.Delete(saveDir, true);
            }
            catch (DirectoryNotFoundException) { }
        }

        [UnityTest]
        public IEnumerable EmptySaveTest()
        {
            var microCosmos = GameObject.Find("Environment").GetComponent<MicroCosmosPersistence>();
            microCosmos.SaveDirectory = saveDir;
            microCosmos.OnSave();

            Assert.AreEqual(new[] {$"{saveDir}\\CellColony-2.json"}, Directory.GetFiles(saveDir));

            yield return null;
        }

        [UnityTest]
        public IEnumerator SaveLoadTest()
        {
            var microCosmos = GameObject.Find("Environment").GetComponent<MicroCosmosPersistence>();
            microCosmos.SaveDirectory = saveDir;
            var chemicalSink = GameObject.Find("Environment").GetComponent<ChemicalSink>();
            var graphManager = GameObject.Find("CellColony").GetComponent<GenealogyGraphManager>();
            var cellColony = GameObject.Find("CellColony").GetComponent<CellColony>();

            EnvironmentInitializer.LoadMicroCosmosFromResources(
                new Dictionary<string, string>
                {
                    {"Environment.CellColony", $"{nameof(MicroCosmosPersistenceTest)}-{nameof(CellColony)}"},
                    {"Environment.ChemicalSink", $"{nameof(MicroCosmosPersistenceTest)}-{nameof(ChemicalSink)}"},
                    {
                        "Environment.GenealogyGraphManager",
                        $"{nameof(MicroCosmosPersistenceTest)}-{nameof(GenealogyGraphManager)}"
                    }
                }, out var resourceTexts
            );

            yield return null;

            Assert.AreEqual(2, cellColony.transform.Children().Count());
            Assert.AreEqual(5, graphManager.genealogyGraph.NodeCount);
            Assert.AreEqual(4, graphManager.genealogyGraph.RelationCount);
            Assert.AreEqual(254.19635f, cellColony.transform.Find("Cell.1").rotation.eulerAngles.z);
            Assert.That(chemicalSink.TotalMass, Is.InRange(.5f, .5f + 1e-6));

            microCosmos.OnSave();

            // Debug.Log(Serialization.ReadAllCompressedText(SubsystemsPersistence.GetSavePath(saveDir, chemicalSink)));
            Assert.AreEqual(
                Resources.Load<TextAsset>($"{nameof(MicroCosmosPersistenceTest)}-{nameof(ChemicalSink)}-after").text,
                Serialization.ReadAllCompressedText(SubsystemsPersistence.GetSavePath(saveDir, chemicalSink)));
            Assert.AreEqual(resourceTexts["Environment.GenealogyGraphManager"],
                Serialization.ReadAllCompressedText(SubsystemsPersistence.GetSavePath(saveDir, graphManager)));
            Assert.AreEqual(resourceTexts["Environment.CellColony"],
                Serialization.ReadAllCompressedText(SubsystemsPersistence.GetSavePath(saveDir, cellColony)));
        }
    }
}