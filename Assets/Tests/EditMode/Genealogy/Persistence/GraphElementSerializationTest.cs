using Genealogy;
using Genealogy.Persistence;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests.EditMode.Genealogy.Persistence
{
    public class GraphElementSerializationTest
    {
        // private static readonly Guid Guid1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        // private static readonly Guid Guid2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        // private static readonly DateTime Jan1 = new DateTime(2020, 1, 1, 1, 0, 0);

        [Test]
        public void TestNodeConversion()
        {
            // var node = new Node(Guid1, NodeType.Reproduction, Jan1);
            var expected = @"{
  ""NodeType"": 1,
  ""Guid"": ""11111111-1111-1111-1111-111111111111"",
  ""CreatedAt"": ""2020-01-01T01:00:00""
}";
            var deserialized = JsonConvert.DeserializeObject<Node>(expected);
            Assert.AreEqual(expected, JsonConvert.SerializeObject(deserialized, Formatting.Indented));
        }

        [Test]
        public void TestReproductionNodeSerialization()
        {
            // var node = new Reproduction(Guid1, Jan1);
            var expected = @"{
  ""NodeType"": 1,
  ""Guid"": ""11111111-1111-1111-1111-111111111111"",
  ""CreatedAt"": ""2020-01-01T01:00:00""
}";
            var deserialized = JsonConvert.DeserializeObject<Reproduction>(expected);
            Assert.AreEqual(expected, JsonConvert.SerializeObject(deserialized, Formatting.Indented));
        }

        [Test]
        public void TestCellNodeSerialization()
        {
            // var node = new CellNode(Guid1, Jan1, "cN1");
            var expected = @"{
  ""displayName"": ""cN1"",
  ""NodeType"": 0,
  ""Guid"": ""11111111-1111-1111-1111-111111111111"",
  ""CreatedAt"": ""2020-01-01T01:00:00""
}";
            var deserialized = JsonConvert.DeserializeObject<CellNode>(expected);
            Assert.AreEqual(expected, JsonConvert.SerializeObject(deserialized, Formatting.Indented));
        }

        [Test]
        public void TestRelationSerialization()
        {
            // var relation = new Relation(
            //     NodeResolver(Guid1),
            //     RelationType.Reproduction,
            //     NodeResolver(Guid2)
            // );

            var expected = @"{
  ""From"": {
    ""GUID"": ""11111111-1111-1111-1111-111111111111""
  },
  ""RelationType"": 0,
  ""To"": {
    ""GUID"": ""22222222-2222-2222-2222-222222222222""
  },
  ""DateTime"": ""2020-11-29T15:25:50.9801908+05:30""
}";
            var deserialized = JsonConvert.DeserializeObject<Relation>(expected, new JsonSerializerSettings()
            {
                ContractResolver = new NodeAsGuidContract()
            });
            // Assert.AreSame(NodeResolver(Guid1), deserialized.From);
            // Assert.AreSame(NodeResolver(Guid2), deserialized.To);
            var actual = JsonConvert.SerializeObject(deserialized, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new NodeAsGuidContract()
            });
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestHeterogeneousNodeArraySerialization()
        {
            // var array = new[]
            // {
            //     new Node(Guid1, NodeType.Cell, Jan1),
            //     new Reproduction(Guid1, Jan1),
            //     new CellNode(Guid1, Jan1, "cell_1")
            // };
            var expected = @"[
  {
    ""NodeType"": 0,
    ""Guid"": ""11111111-1111-1111-1111-111111111111"",
    ""CreatedAt"": ""2020-01-01T01:00:00""
  },
  {
    ""$type"": ""Genealogy.Reproduction, MicroCosmosScripts"",
    ""NodeType"": 1,
    ""Guid"": ""11111111-1111-1111-1111-111111111111"",
    ""CreatedAt"": ""2020-01-01T01:00:00""
  },
  {
    ""$type"": ""Genealogy.CellNode, MicroCosmosScripts"",
    ""displayName"": ""cell_1"",
    ""NodeType"": 0,
    ""Guid"": ""11111111-1111-1111-1111-111111111111"",
    ""CreatedAt"": ""2020-01-01T01:00:00""
  }
]";
            var deserialized = JsonConvert.DeserializeObject<Node[]>(expected, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            var actual = JsonConvert.SerializeObject(deserialized, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            });
            Assert.AreEqual(expected, actual);
        }
    }
}