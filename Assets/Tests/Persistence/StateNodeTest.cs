using System.Data;
using Newtonsoft.Json;
using NUnit.Framework;
using Persistence;

namespace Tests.Persistence
{
    public class StateNodeTest
    {
        [Test]
        public void TestSerializationDeserialization()
        {
            var expectedJson =
                @"{""state"":{""x"":1,""y"":2},""children"":[{""state"":{""name"":""Bob""},""children"":[]},{""state"":{""blah"":{""more"":""junk""}},""children"":[]}]}";
            var node = JsonConvert.DeserializeObject<StateNode>(expectedJson);
            Assert.AreEqual(expectedJson, JsonConvert.SerializeObject(node, Formatting.None));
        }

        [Test]
        public void TestSaveLoad()
        {
            var expectedJson =
                @"{""state"":{""x"":1,""y"":2},""children"":[{""state"":{""name"":""Bob""},""children"":[]},{""state"":{""blah"":{""more"":""junk""}},""children"":[]}]}";
            var livingComponent = new SampleLivingComponent();
            StateNode.Load(livingComponent, JsonConvert.DeserializeObject<StateNode>(expectedJson));
            var node = StateNode.Save(livingComponent);
            Assert.AreEqual(expectedJson, JsonConvert.SerializeObject(node, Formatting.None));
        }

        [Test]
        public void TestMismatchingSubLengths()
        {
            var expectedJson =
                @"{""state"":{""x"":1,""y"":2},""children"":[{""state"":{""name"":""Bob""},""children"":[]}]}";
            var livingComponent = new SampleLivingComponent();
            Assert.Throws<DataException>(() =>
                StateNode.Load(livingComponent, JsonConvert.DeserializeObject<StateNode>(expectedJson)));
        }
    }
}