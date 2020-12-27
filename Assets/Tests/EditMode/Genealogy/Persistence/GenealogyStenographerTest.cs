using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Genealogy.Graph;
using Genealogy.Persistence;
using NUnit.Framework;
using UnityEngine;

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

        private static readonly string RootNodeJson = @"[
  {
    ""$type"": ""Genealogy.Persistence.GenealogyScrollRootEntry, MicroCosmosScripts"",
    ""Node"": {
      ""$type"": ""Genealogy.Graph.CellNode, MicroCosmosScripts"",
      ""displayName"": ""Cell"",
      ""NodeType"": 0,
      ""Guid"": ""00000000-0000-0000-0000-000000000000"",
      ""CreatedAt"": ""2020-01-01T01:00:00""
    }
  }]";

        private static readonly string RelationJson = @"[
  {
    ""$type"": ""Genealogy.Persistence.GenealogyScrollRootEntry, MicroCosmosScripts"",
    ""Node"": {
      ""$type"": ""Genealogy.Graph.CellNode, MicroCosmosScripts"",
      ""displayName"": ""Cell"",
      ""NodeType"": 0,
      ""Guid"": ""00000000-0000-0000-0000-000000000000"",
      ""CreatedAt"": ""2020-01-01T01:00:00""
    }
  },
  {
    ""$type"": ""Genealogy.Persistence.GenealogyScrollEntry, MicroCosmosScripts"",
    ""Node"": {
      ""$type"": ""Genealogy.Graph.Reproduction, MicroCosmosScripts"",
      ""NodeType"": 1,
      ""Guid"": ""00000000-1111-1111-1111-111111111111"",
      ""CreatedAt"": ""2020-01-01T01:00:01""
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
  }]";

        [Test]
        public void TestUnopenedScroll()
        {
            var serialized = DoStenography(new GenealogyGraph(), _ => { });
            Assert.AreEqual("[]", serialized);
        }

        [Test]
        public void TestRootNodeScrollSerialization()
        {
            var graph = new GenealogyGraph();
            var serialized = DoStenography(graph, _ => graph.RegisterRootNode(Root));
            Assert.AreEqual(RootNodeJson, serialized);
        }

        [Test]
        public void TestRelationSerialization()
        {
            var graph = new GenealogyGraph();
            var serialized = DoStenography(graph, _ =>
            {
                graph.RegisterRootNode(Root);
                graph.RegisterReproduction(new[] {Root}, new Reproduction(RGuid1, Root.CreatedAt + OneSec));
            });
            Assert.AreEqual(RelationJson, serialized);
        }

        [Test]
        public void TestRootNodeReproducibility()
        {
            var g1 = new GenealogyGraph();
            IEnumerable<GenealogyScrollEntryBase> entries1 = null;
            var serialized = DoStenography(g1, stenographer =>
            {
                g1.RegisterRootNode(Root);
                entries1 = stenographer.ReadAll();
            });

            var g2 = new GenealogyGraph();
            var serialized2 = DoStenography(g2, _ => new ScrollReader(g2).Load(entries1));

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
            IEnumerable<GenealogyScrollEntryBase> entries1 = null;
            var serialized = DoStenography(g1, stenographer =>
            {
                g1.RegisterRootNode(Root);
                var cell1 = new CellNode(Guid1, Root.CreatedAt + OneSec, "Cell1");
                g1.RegisterReproductionAndOffspring(new[] {Root}, (Node) cell1);
                entries1 = stenographer.ReadAll();
            });

            var g2 = new GenealogyGraph();
            var serialized2 = DoStenography(g2, _ => new ScrollReader(g2).Load(entries1));

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
            IEnumerable<GenealogyScrollEntryBase> entries1 = null;
            var serialized = DoStenography(g1, stenographer =>
            {
                g1.RegisterRootNode(Root);
                var cell1 = new CellNode(Guid1, Root.CreatedAt + OneSec, "Cell1");
                g1.RegisterReproductionAndOffspring(new[] {Root}, (Node) cell1);

                var cell1RootRep = new Reproduction(RGuid1, cell1.CreatedAt + OneSec);
                g1.RegisterReproduction(new[] {cell1, Root}, cell1RootRep);
                var cell2 = new CellNode(Guid2, cell1RootRep.CreatedAt + OneTick, "cell2");
                g1.RegisterOffspring(cell1RootRep, cell2);
                var cell3 = new CellNode(Guid3, cell2.CreatedAt + OneTick, "cell3");
                g1.RegisterOffspring(cell1RootRep, cell3);
                var cell2Cell3Rep = new Reproduction(RGuid2, cell3.CreatedAt + OneSec);
                g1.RegisterReproduction(new Node[] {cell2, cell3}, cell2Cell3Rep);
                var cell4 = new CellNode(Guid4, cell2Cell3Rep.CreatedAt + OneTick, "cell4");
                g1.RegisterOffspring(cell1RootRep, cell4);
                entries1 = stenographer.ReadAll();
            });

            var g2 = new GenealogyGraph();
            var serialized2 = DoStenography(g2, _ => new ScrollReader(g2).Load(entries1));

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

        private string DoStenography(GenealogyGraph graph, Action<ScrollStenographer> action)
        {
            var stenographer = new ScrollStenographer();
            graph.AddListener(stenographer);

            try
            {
                action(stenographer);
                var tmpFile = $"{Application.temporaryCachePath}/testing/GenealogyStenographerTest/tmp.json";
                stenographer.SaveCopy(tmpFile);
                return File.ReadAllText(tmpFile);
            }
            finally
            {
                stenographer.CloseScroll();
            }
        }
    }
}