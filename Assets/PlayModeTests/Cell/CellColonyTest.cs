using System.Collections;
using System.IO;
using System.Linq;
using Cell;
using Genetics;
using Newtonsoft.Json;
using NUnit.Framework;
using Persistence;
using TestCommon;
using Tests.Genetics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PlayModeTests.Cell
{
    public class CellColonyTest
    {
        private string saveDir;

        [OneTimeSetUp]
        public void GeneNodeTestSimplePasses()
        {
            SceneManager.LoadScene("PlayModeTests/Cell/CellColonyTestScene");

            saveDir = $"{Application.temporaryCachePath}/testing/CellColonyTest";
            Directory.CreateDirectory(saveDir);
        }

        [UnityTest]
        public IEnumerator SaveLoadTest()
        {
            var loadFile = $"{saveDir}/testLoad.json";
            using (var streamWriter = File.CreateText(loadFile))
            {
                var saveResource = Resources.Load<TextAsset>("CellColony-testSave");
                streamWriter.Write(saveResource.text);
            }

            var cellColony = GameObject.Find("CellColony").GetComponent<CellColony>();
            cellColony.saveFile = loadFile;
            cellColony.OnLoad();

            Assert.AreEqual(2, cellColony.transform.Children().Count());
            Assert.AreEqual(254.19635f, cellColony.transform.Find("Cell1[0]").rotation.eulerAngles.z);

            cellColony.saveFile = $"{saveDir}/testSave.json";
            cellColony.OnSave();
            Assert.IsTrue(File.ReadAllBytes(loadFile).SequenceEqual(File.ReadAllBytes(cellColony.saveFile)));

            yield return null;
        }
    }
}