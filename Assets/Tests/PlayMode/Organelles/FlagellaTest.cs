using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Environment;
using NUnit.Framework;
using Organelles.Flagella;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayMode.Organelles
{
    public class FlagellaTest
    {
        [OneTimeSetUp]
        public void Setup() => SceneManager.LoadScene("Tests/PlayMode/CellColonyTestScene");

        [UnityTest]
        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        public IEnumerator TestFlagellaActuatorCausesCellMovement()
        {
            EnvironmentInitializer.LoadEnvironmentFromResources(
                "ActuatorTest-testSave", out _,
                "ActuatorTest-testChemicalSinkSaveFile", out _,
                "ActuatorTest-testGenealogyScroll", out _
            );

            var cell = GameObject.Find("Cell.1").GetComponent<global::Cell.Cell>();
            var pos0 = cell.transform.position;
            var angle0 = cell.transform.rotation.eulerAngles.z;
            var flagella = cell.GetComponentInChildren<FlagellaActuator>();
            var logits = flagella.Connect();
            logits[0] = .04f;
            logits[1] = .1f;
            yield return null;
            flagella.Actuate(logits);
            yield return new WaitForSeconds(.1f);
            Assert.Greater(Vector2.Dot(cell.transform.position - pos0, cell.transform.up), 0f, "Cell moved forward");
            Assert.Greater(cell.transform.rotation.eulerAngles.z - angle0, 0, "Cell turned right");

            logits[0] = -.8f;
            logits[1] = -.9f;
            flagella.Actuate(logits);
            yield return new WaitForSeconds(.2f);
            Assert.Less(Vector2.Dot(cell.transform.position - pos0, cell.transform.up), 0f, "Cell moved backward");
            Assert.Less(cell.transform.rotation.eulerAngles.z - angle0, 0, "Cell turned left");
        }
    }
}