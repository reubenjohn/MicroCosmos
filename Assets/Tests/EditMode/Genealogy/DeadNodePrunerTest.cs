using System;
using Genealogy;
using Genealogy.Graph;
using NUnit.Framework;

namespace Tests.EditMode.Genealogy
{
    public class DeadNodePrunerTest
    {
        private static readonly Node Root = new Node(Guid.Parse("00000000-0000-0000-0000-000000000000"), NodeType.Cell,
            new DateTime(2020, 1, 1));

        private static readonly Guid Guid1 = Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e");
        private static readonly Guid Guid2 = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7");
        private static readonly Guid Guid3 = Guid.Parse("e07fc1f9-d9cb-40de-a165-70867728950e");

        [Test]
        public void TestRootNode()
        {
            var graph = new GenealogyGraph();
            var livingFilter = new DeadNodePruner();
            graph.AddListener(livingFilter);

            graph.RegisterRootNode(Root);
            Assert.AreEqual(1, graph.NodeCount);
            Assert.AreEqual(0, graph.RelationCount);
        }

        [Test]
        public void TestLinear2GenerationLayout()
        {
            var graph = new GenealogyGraph();
            var livingFilter = new DeadNodePruner();
            graph.AddListener(livingFilter);

            graph.RegisterRootNode(Root);
            var node00 = new CellNode(Guid1, Root.CreatedAt + TimeSpan.FromTicks(2), "00");
            graph.RegisterReproductionAndOffspring(new[] {Root}, node00);
            Assert.AreEqual(3, graph.NodeCount);
            Assert.AreEqual(2, graph.RelationCount);

            graph.RegisterDeath(node00, new CellDeath(Guid.NewGuid()));
            Assert.AreEqual(1, graph.NodeCount);
            Assert.AreEqual(0, graph.RelationCount);
        }

        [Test]
        public void TestTwoChildGenealogyLayout()
        {
            var graph = new GenealogyGraph();
            var livingFilter = new DeadNodePruner();
            graph.AddListener(livingFilter);

            graph.RegisterRootNode(Root);
            var node11 = new CellNode(Guid1, Root.CreatedAt + TimeSpan.FromTicks(2), "11");
            var node12 = new CellNode(Guid2, Root.CreatedAt + TimeSpan.FromTicks(4), "12");
            graph.RegisterReproductionAndOffspring(new[] {Root}, node11);
            graph.RegisterReproductionAndOffspring(new[] {Root}, node12);
//   00
// 00  01
// 00  00

            Assert.AreEqual(5, graph.NodeCount);
            Assert.AreEqual(4, graph.RelationCount);

            graph.RegisterDeath(node11, new CellDeath(Guid.NewGuid()));
            Assert.AreEqual(3, graph.NodeCount);
            Assert.AreEqual(2, graph.RelationCount);
        }

        [Test]
        public void TestParentDeadButHasOtherChildren()
        {
            var graph = new GenealogyGraph();
            var livingFilter = new DeadNodePruner();
            graph.AddListener(livingFilter);

            graph.RegisterRootNode(Root);
            var node11 = new CellNode(Guid1, Root.CreatedAt + TimeSpan.FromTicks(2), "11");
            var node12 = new CellNode(Guid2, Root.CreatedAt + TimeSpan.FromTicks(4), "12");
            var node111 = new CellNode(Guid3, Root.CreatedAt + TimeSpan.FromTicks(6), "111");
            graph.RegisterReproductionAndOffspring(new[] {Root}, node11);
            graph.RegisterReproductionAndOffspring(new[] {Root}, node12);
            graph.RegisterReproductionAndOffspring(new Node[] {node11}, node111);

            Assert.AreEqual(7, graph.NodeCount);
            Assert.AreEqual(6, graph.RelationCount);

            graph.RegisterDeath(node11, new CellDeath(Guid.NewGuid(), Root.CreatedAt + TimeSpan.FromTicks(7)));
            Assert.AreEqual(8, graph.NodeCount);
            Assert.AreEqual(7, graph.RelationCount);

            graph.RegisterDeath(node111, new CellDeath(Guid.NewGuid(), Root.CreatedAt + TimeSpan.FromTicks(8)));
            Assert.AreEqual(3, graph.NodeCount);
            Assert.AreEqual(2, graph.RelationCount);
        }

        [Test]
        public void TestParentAlive()
        {
            var graph = new GenealogyGraph();
            var livingFilter = new DeadNodePruner();
            graph.AddListener(livingFilter);

            graph.RegisterRootNode(Root);
            var node11 = new CellNode(Guid1, Root.CreatedAt + TimeSpan.FromTicks(2), "11");
            var node12 = new CellNode(Guid2, Root.CreatedAt + TimeSpan.FromTicks(4), "12");
            var node111 = new CellNode(Guid3, Root.CreatedAt + TimeSpan.FromTicks(6), "111");
            graph.RegisterReproductionAndOffspring(new[] {Root}, node11);
            graph.RegisterReproductionAndOffspring(new[] {Root}, node12);
            graph.RegisterReproductionAndOffspring(new Node[] {node11}, node111);

            Assert.AreEqual(7, graph.NodeCount);
            Assert.AreEqual(6, graph.RelationCount);

            graph.RegisterDeath(node111, new CellDeath(Guid.NewGuid(), Root.CreatedAt + TimeSpan.FromTicks(7)));
            Assert.AreEqual(5, graph.NodeCount);
            Assert.AreEqual(4, graph.RelationCount);
        }
    }
}