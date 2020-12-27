using System.Data;
using Newtonsoft.Json;
using NUnit.Framework;
using Persistence;
using UnityEngine.SceneManagement;

namespace Tests.PlayMode.Persistence
{
    public class StateNodeTest
    {
        private static readonly string EXPECTED_JSON =
            @"{""state"":{""x"":1,""y"":2},""children"":[{""state"":{""name"":""Bob""},""children"":[]},{""state"":{""blah"":{""more"":""junk""}},""children"":[]}]}";

        private SampleLivingComponent livingComponent;

        [OneTimeSetUp]
        public void Setup()
        {
            SceneManager.LoadScene("Tests/PlayMode/GeneralTestScene");
            var geneNode = JsonConvert.DeserializeObject<GeneNode>(GeneNodeTest.Serialized1);
            livingComponent = GeneNode.Load(geneNode, null).GetComponent<SampleLivingComponent>();
        }

        [Test]
        public void TestSerializationDeserialization()
        {
            var node = JsonConvert.DeserializeObject<StateNode>(EXPECTED_JSON);
            Assert.AreEqual(EXPECTED_JSON, JsonConvert.SerializeObject(node, Formatting.None));
        }

        [Test]
        public void TestSaveLoad()
        {
            StateNode.Load(livingComponent, JsonConvert.DeserializeObject<StateNode>(EXPECTED_JSON));
            var node = StateNode.Save(livingComponent);
            Assert.AreEqual(EXPECTED_JSON, JsonConvert.SerializeObject(node, Formatting.None));
        }

        [Test]
        public void TestMismatchingSubLengths()
        {
            var expectedJson =
                @"{""state"":{""x"":1,""y"":2},""children"":[{""state"":{""name"":""Bob""},""children"":[]}]}";
            Assert.Throws<DataException>(() =>
                StateNode.Load(livingComponent, JsonConvert.DeserializeObject<StateNode>(expectedJson)));
        }
    }
}