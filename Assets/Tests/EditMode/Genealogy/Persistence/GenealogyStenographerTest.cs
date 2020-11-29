﻿using System;
using System.IO;
using System.Linq;
using Genealogy;
using Genealogy.Persistence;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests.EditMode.Genealogy.Persistence
{
    public class GenealogyStenographerTest
    {
        private static readonly TimeSpan OneSec = new TimeSpan(0, 0, 1);
        private static readonly TimeSpan OneTick = new TimeSpan(1);

        private static readonly Node Root = new CellNode(Guid.Parse("00000000-0000-0000-0000-000000000000"),
            new DateTime(2020, 1, 1, 1, 0, 0), "Cell");

        private static readonly Guid Guid1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid Guid2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid Guid3 = Guid.Parse("33333333-3333-3333-3333-333333333333");
        private static readonly Guid Guid4 = Guid.Parse("44444444-4444-4444-4444-444444444444");
        private static readonly Guid Guid5 = Guid.Parse("55555555-5555-5555-5555-555555555555");

        private static readonly Guid RGuid1 = Guid.Parse("00000000-1111-1111-1111-111111111111");
        private static readonly Guid RGuid2 = Guid.Parse("00000000-2222-2222-2222-222222222222");

        private static readonly string RootNodeJson = @"{""rootEntry"": {
    ""RootNode"": {
      ""$type"": ""Genealogy.CellNode, MicroCosmosScripts"",
      ""displayName"": ""Cell"",
      ""NodeType"": 0,
      ""Guid"": ""00000000-0000-0000-0000-000000000000"",
      ""RegistrationTime"": ""2020-01-01T01:00:00""
    }
  },""entries"":[]}";

        private static readonly string RelationJson = @"{""rootEntry"": {
    ""RootNode"": {
      ""$type"": ""Genealogy.CellNode, MicroCosmosScripts"",
      ""displayName"": ""Cell"",
      ""NodeType"": 0,
      ""Guid"": ""00000000-0000-0000-0000-000000000000"",
      ""RegistrationTime"": ""2020-01-01T01:00:00""
    }
  },""entries"":[
    {
      ""Node"": {
        ""$type"": ""Genealogy.Reproduction, MicroCosmosScripts"",
        ""NodeType"": 1,
        ""Guid"": ""00000000-1111-1111-1111-111111111111"",
        ""RegistrationTime"": ""2020-01-01T01:00:01""
      },
      ""Relations"": [
        {
          ""From"": {
            ""GUID"": ""00000000-0000-0000-0000-000000000000""
          },
          ""RelationType"": 0,
          ""To"": {
            ""GUID"": ""00000000-1111-1111-1111-111111111111""
          },
          ""DateTime"": ""2020-01-01T01:00:01""
        }
      ]
    }]}";

        [Test]
        public void TestUnopenedScroll()
        {
            var serialized = DoStenography(new GenealogyGraph(), () => { });
            Assert.AreEqual("{}", serialized);
        }

        [Test]
        public void TestRootNodeScrollSerialization()
        {
            var graph = new GenealogyGraph();
            var serialized = DoStenography(graph, () => graph.RegisterRootNode(Root));
            Assert.AreEqual(RootNodeJson, serialized);
        }

        [Test]
        public void TestRelationSerialization()
        {
            var graph = new GenealogyGraph();
            var serialized = DoStenography(graph, () =>
            {
                graph.RegisterRootNode(Root);
                graph.RegisterReproduction(new[] {Root}, new Reproduction(RGuid1, Root.RegistrationTime + OneSec));
            });
            Assert.AreEqual(RelationJson, serialized);
        }

        [Test]
        public void TestRootNodeReproducibility()
        {
            var g1 = new GenealogyGraph();
            var serialized = DoStenography(g1, () => g1.RegisterRootNode(Root));

            var g2 = new GenealogyGraph();
            var sr = new StringReader(serialized);
            var serialized2 = DoStenography(g2, () => ScrollReader.Load(new JsonTextReader(sr), g2));

            Assert.AreEqual(serialized, serialized2);

            // Assert.IsNull(deserialized.rootEntry.RootNode, "Loader should dispose deserialized objects when done");
            // Assert.IsEmpty(deserialized.entries);

            Assert.AreEqual(1, g2.NodeCount);
            Assert.AreEqual(g1.rootNode.Guid, g2.rootNode.Guid);
            Assert.IsInstanceOf<CellNode>(g2.rootNode);

            Assert.AreEqual(0, g2.RelationCount);
        }

        [Test]
        public void TestAsexualReproductionReproducibility()
        {
            var g1 = new GenealogyGraph();
            var serialized = DoStenography(g1, () =>
            {
                g1.RegisterRootNode(Root);
                var cell1 = new CellNode(Guid1, Root.RegistrationTime + OneSec, "Cell1");
                g1.RegisterReproductionAndOffspring(new[] {Root}, (Node) cell1);
            });

            var g2 = new GenealogyGraph();
            var sr = new StringReader(serialized);
            var serialized2 = DoStenography(g2, () => ScrollReader.Load(new JsonTextReader(sr), g2));

            Assert.AreEqual(serialized, serialized2);

            Assert.AreEqual(3, g2.NodeCount);
            Assert.AreEqual(g1.rootNode.Guid, g2.rootNode.Guid);
            Assert.IsInstanceOf<CellNode>(g2.rootNode);

            Assert.AreEqual(2, g2.RelationCount);
            var relations = g2.Relations.ToArray();
            Assert.AreSame(g2.GetNode(relations[0].From.Guid), relations[0].From);
            Assert.AreSame(g2.GetNode(relations[0].To.Guid), relations[0].To);
            Assert.AreSame(g2.GetNode(relations[1].From.Guid), relations[1].From);
            Assert.AreSame(g2.GetNode(relations[1].To.Guid), relations[1].To);
        }

        [Test]
        public void TestHeavyIncestReproducibility()
        {
            var g1 = new GenealogyGraph();
            var serialized = DoStenography(g1, () =>
            {
                g1.RegisterRootNode(Root);
                var cell1 = new CellNode(Guid1, Root.RegistrationTime + OneSec, "Cell1");
                g1.RegisterReproductionAndOffspring(new[] {Root}, (Node) cell1);

                var cell1RootRep = new Reproduction(RGuid1, cell1.RegistrationTime + OneSec);
                g1.RegisterReproduction(new[] {cell1, Root}, cell1RootRep);
                var cell2 = new CellNode(Guid2, cell1RootRep.RegistrationTime + OneTick, "cell2");
                g1.RegisterOffspring(cell1RootRep, cell2);
                var cell3 = new CellNode(Guid3, cell2.RegistrationTime + OneTick, "cell3");
                g1.RegisterOffspring(cell1RootRep, cell3);
                var cell2Cell3Rep = new Reproduction(RGuid2, cell3.RegistrationTime + OneSec);
                g1.RegisterReproduction(new Node[] {cell2, cell3}, cell2Cell3Rep);
                var cell4 = new CellNode(Guid4, cell2Cell3Rep.RegistrationTime + OneTick, "cell4");
                g1.RegisterOffspring(cell1RootRep, cell4);
            });

            var g2 = new GenealogyGraph();
            var sr = new StringReader(serialized);
            var serialized2 = DoStenography(g2, () => ScrollReader.Load(new JsonTextReader(sr), g2));

            Assert.AreEqual(serialized, serialized2);

            Assert.AreEqual(8, g2.NodeCount);
            Assert.AreEqual(g1.rootNode.Guid, g2.rootNode.Guid);
            Assert.IsInstanceOf<CellNode>(g2.rootNode);

            Assert.AreEqual(9, g2.RelationCount);
            foreach (var relation in g2.Relations.ToArray())
            {
                Assert.AreSame(g2.GetNode(relation.From.Guid), relation.From);
                Assert.AreSame(g2.GetNode(relation.To.Guid), relation.To);
            }
        }

        private string DoStenography(GenealogyGraph graph, Action action)
        {
            StringWriter sw = null;
            var stenographer = new ScrollStenographer(() => new JsonTextWriter(sw = new StringWriter()));
            graph.AddListener(stenographer);

            action.Invoke();

            stenographer.CloseScroll();
            return sw?.ToString();
        }
    }
}