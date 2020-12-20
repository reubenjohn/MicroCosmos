using System.Collections;
using System.IO;
using System.Linq;
using Environment;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Util;

namespace Tests.PlayMode.Cell
{
    public class CellColonyTest
    {
        [OneTimeSetUp]
        public void GeneNodeTestSimplePasses()
        {
            SceneManager.LoadScene("Tests/PlayMode/CellColonyTestScene");
        }

        [UnityTest]
        public IEnumerable EmptySaveTest()
        {
            var cellColony = GameObject.Find("CellColony").GetComponent<CellColony>();
            var graphManager = GameObject.Find("CellColony").GetComponent<GenealogyGraphManager>();
            var chemicalSink = GameObject.Find("Environment").GetComponent<ChemicalSink>();
            cellColony.SaveDirectory = $"{Application.temporaryCachePath}/testing/CellColonyTest";
            cellColony.saveFile = "testSave";
            chemicalSink.saveFile = "testSaveChemicalSink";
            graphManager.saveFile = "testSaveGenealogy";
            Assert.AreEqual($"{Application.temporaryCachePath}/testing/CellColonyTest/testSave.json",
                cellColony.SavePath);
            Assert.AreEqual($"{Application.temporaryCachePath}/testing/CellColonyTest/testSaveChemicalSink.json",
                chemicalSink.PersistenceFilePath(cellColony.SaveDirectory));
            Assert.AreEqual($"{Application.temporaryCachePath}/testing/CellColonyTest/testSaveGenealogy.json",
                graphManager.PersistenceFilePath(cellColony.SaveDirectory));
            try
            {
                Directory.Delete(cellColony.SaveDirectory, true);
            }
            catch (DirectoryNotFoundException) { }

            cellColony.OnSave();
            Assert.IsTrue(File.Exists($"{Application.temporaryCachePath}/testing/CellColonyTest/testSave.json"),
                "Saving creates a test folder when it doesn't already exist");
            Assert.IsTrue(
                File.Exists($"{Application.temporaryCachePath}/testing/CellColonyTest/testSaveChemicalSink.json"),
                "Saving creates a test folder when it doesn't already exist");
            Assert.IsTrue(
                File.Exists($"{Application.temporaryCachePath}/testing/CellColonyTest/testSaveGenealogy.json"),
                "Saving creates a test folder when it doesn't already exist");

            Directory.Delete(cellColony.SaveDirectory, true);
            yield return null;
        }

        [UnityTest]
        public IEnumerator SaveLoadTest()
        {
            var cellColony = GameObject.Find("CellColony").GetComponent<CellColony>();
            var graphManager = GameObject.Find("CellColony").GetComponent<GenealogyGraphManager>();
            var chemicalSink = GameObject.Find("Environment").GetComponent<ChemicalSink>();

            EnvironmentInitializer.LoadEnvironmentFromResources(
                "CellColony-testSave", out var cellColonyExpected,
                "CellColony-testChemicalSinkSaveFile", out _,
                "CellColony-testGenealogyScroll", out var genealogyExpected
            );

            yield return null;

            Assert.AreEqual(2, cellColony.transform.Children().Count());
            Assert.AreEqual(254.19635f, cellColony.transform.Find("Cell.1").rotation.eulerAngles.z);
            Assert.That(chemicalSink.TotalMass, Is.InRange(.5f, .5f + 1e-6));

            cellColony.SaveDirectory = $"{Application.temporaryCachePath}/testing/CellColonyTest";

            cellColony.OnSave();

            Debug.Log(graphManager.PersistenceFilePath(cellColony.SaveDirectory));
            Assert.AreEqual(cellColonyExpected, File.ReadAllText(cellColony.SavePath));
            Assert.AreEqual("{\r\n  \"Fat\": 0.5\r\n}",
                File.ReadAllText(chemicalSink.PersistenceFilePath(cellColony.SaveDirectory)));
            Assert.AreEqual(genealogyExpected,
                File.ReadAllText(graphManager.PersistenceFilePath(cellColony.SaveDirectory)));

            Directory.Delete(cellColony.SaveDirectory, true);
        }
    }
}