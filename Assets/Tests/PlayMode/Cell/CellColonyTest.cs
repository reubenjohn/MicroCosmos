using System.Collections;
using System.IO;
using System.Linq;
using Cell;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayMode.Cell
{
    public class CellColonyTest
    {
        [OneTimeSetUp]
        public void GeneNodeTestSimplePasses()
        {
            SceneManager.LoadScene("Tests/PlayMode/Cell/CellColonyTestScene");
        }

        [UnityTest]
        public IEnumerator SaveLoadTest()
        {
            var cellColony = GameObject.Find("CellColony").GetComponent<CellColony>();
            cellColony.SaveDirectory = $"{Application.temporaryCachePath}/testing/CellColonyTest";
            cellColony.saveFile = "testSave";
            Assert.AreEqual($"{Application.temporaryCachePath}/testing/CellColonyTest/testSave.json",
                cellColony.SavePath);
            try
            {
                Directory.Delete(cellColony.SaveDirectory, true);
            }
            catch (DirectoryNotFoundException)
            {
            }

            cellColony.OnSave();
            Assert.IsTrue(File.Exists($"{Application.temporaryCachePath}/testing/CellColonyTest/testSave.json"),
                "Saving creates a test folder when it doesn't already exist");

            cellColony.saveFile = "testLoad";
            var loadFile = cellColony.SavePath;
            using (var streamWriter = File.CreateText(loadFile))
            {
                var saveResource = Resources.Load<TextAsset>("CellColony-testSave");
                streamWriter.Write(saveResource.text);
            }

            cellColony.OnLoad();

            Assert.AreEqual(2, cellColony.transform.Children().Count());
            Assert.AreEqual(254.19635f, cellColony.transform.Find("Cell1[0]").rotation.eulerAngles.z);

            cellColony.saveFile = "testSave";
            cellColony.OnSave();
            Assert.AreEqual(File.ReadAllText(loadFile), File.ReadAllText(cellColony.SavePath));

            yield return null;
        }
    }
}