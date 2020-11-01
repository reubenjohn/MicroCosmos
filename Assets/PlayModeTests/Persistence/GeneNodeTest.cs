using System.Collections;
using Genetics;
using Newtonsoft.Json;
using NUnit.Framework;
using Persistence;
using TestCommon;
using Tests.Genetics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace PlayModeTests.Persistence
{
    public class GeneNodeTest
    {
        public static readonly string Serialized1 =
            @"{""resource"":""SampleOrganelle1"",""name"":""MySampleLivingComponent1"",""gene"":" +
            SampleGene.Serialized1 +
            @",""children"":[" +
            @"{""resource"":""SampleSubOrganelle1"",""name"":""MySampleSubLivingComponent1"",""gene"":" +
            SampleSubGene.Serialized1 + @",""children"":[]}," +
            @"{""resource"":""SampleSubOrganelle1"",""name"":""MySampleSubLivingComponent2"",""gene"":" +
            SampleSubGene.Serialized1 + @",""children"":[]}"
            + @"]}";

        [OneTimeSetUp]
        public void GeneNodeTestSimplePasses()
        {
            SceneManager.LoadScene("PlayModeTests/Persistence/EmptyTestScene");
        }

        [UnityTest]
        public IEnumerator SaveLoadTest()
        {
            var geneNode = JsonConvert.DeserializeObject<GeneNode>(Serialized1);
            var gameObject = GeneNode.Load(geneNode, GameObject.Find("Container").transform);
            var livingComponent = gameObject.GetComponent<ILivingComponent>();
            var savedGeneNode = GeneNode.Save(livingComponent);
            Assert.AreEqual(Serialized1, JsonConvert.SerializeObject(savedGeneNode, Formatting.None));
            yield return null;
        }

        [UnityTest]
        public IEnumerator LoadToPositionTest()
        {
            var geneNode = JsonConvert.DeserializeObject<GeneNode>(Serialized1);
            var position = new Vector3(1, 2, 3);
            var rotation = Quaternion.Euler(1, 2, 3);
            var gameObject = GeneNode.Load(geneNode, GameObject.Find("Container").transform, position, rotation);
            Assert.AreEqual(gameObject.transform.position.ToString(), position.ToString());
            Assert.AreEqual(gameObject.transform.rotation, rotation);
            var livingComponent = gameObject.GetComponent<ILivingComponent>();
            var savedGeneNode = GeneNode.Save(livingComponent);
            Assert.AreEqual(Serialized1, JsonConvert.SerializeObject(savedGeneNode, Formatting.None));
            yield return null;
        }

        [UnityTest]
        public IEnumerator MutationTest()
        {
            var geneNode = JsonConvert.DeserializeObject<GeneNode>(Serialized1);
            var gameObject = GeneNode.Load(geneNode, GameObject.Find("Container").transform);
            var livingComponent = gameObject.GetComponent<ILivingComponent>();
            Random.InitState(0);
            var mutated = GeneNode.GetMutated(livingComponent);
            var expectedJson =
                @"{""resource"":""SampleOrganelle1"",""name"":""MySampleLivingComponent1"",""gene"":" +
                @"{""furriness"":0.483172059,""nEyes"":2,""dietaryRestriction"":0,""limbs"":[{""length"":3.34669876},{""length"":3.43899369}]},""children"":["
                + @"{""resource"":""SampleSubOrganelle1"",""name"":""MySampleSubLivingComponent1"",""gene"":{""happiness"":0.9700247},""children"":[]}," +
                @"{""resource"":""SampleSubOrganelle1"",""name"":""MySampleSubLivingComponent2"",""gene"":{""happiness"":0.9694879},""children"":[]}]}";
            Assert.AreEqual(expectedJson, JsonConvert.SerializeObject(mutated, Formatting.None));
            yield return null;
        }
    }
}