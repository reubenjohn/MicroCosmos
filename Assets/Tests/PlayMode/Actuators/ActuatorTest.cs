using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Cell;
using Newtonsoft.Json;
using NUnit.Framework;
using Organelles.Flagella;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayMode.Actuators
{
    public class ActuatorTest
    {
        [OneTimeSetUp]
        public void GeneNodeTestSimplePasses()
        {
            SceneManager.LoadScene("Tests/PlayMode/GeneralTestScene");
        }

        [UnityTest]
        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        public IEnumerator TestFlagellaActuatorCausesCellMovement()
        {
            var container = GameObject.Find("Container").transform;
            var cellDataResource = Resources.Load<TextAsset>("Cell-FlagellaActuator-Test");
            var cell = CellData.Load(JsonConvert.DeserializeObject<CellData>(cellDataResource.text), container);
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

        [OneTimeTearDown]
        public void TearDown()
        {
            Object.Destroy(GameObject.Find("Container"));
        }
    }
}