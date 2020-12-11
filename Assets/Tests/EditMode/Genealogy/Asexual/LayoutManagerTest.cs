using System;
using System.Collections.Generic;
using Genealogy.Graph;
using Genealogy.Layout.Asexual;
using NUnit.Framework;

namespace Tests.EditMode.Genealogy.Asexual
{
    public class LayoutManagerTest
    {
        public static readonly Node Root = new Node(Guid.Parse("00000000-0000-0000-0000-000000000000"), NodeType.Cell,
            new DateTime(2020, 1, 1));

        private static readonly Guid Guid1 = Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e");
        private static readonly Guid Guid2 = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7");
        private static readonly Guid Guid3 = Guid.Parse("e07fc1f9-d9cb-40de-a165-70867728950e");

        [Test]
        public void TestRootNode()
        {
            var tree = new GenealogyGraph();
            var layoutNodes = new Dictionary<Guid, LayoutNode>();
            var layout = new LayoutManager(layoutNodes);
            tree.AddListener(layout);

            tree.RegisterRootNode(Root);
            Assert.AreEqual(1, layoutNodes.Count);
            Assert.AreSame(Root, layoutNodes[Root.Guid].Node);
            Assert.IsNull(layoutNodes[Root.Guid].Parent);
        }

        [Test]
        public void TestLinear2GenerationLayout()
        {
            var tree = new GenealogyGraph();
            var layoutNodes = new Dictionary<Guid, LayoutNode>();
            var layout = new LayoutManager(layoutNodes);
            tree.AddListener(layout);

            tree.RegisterRootNode(Root);
            var node00 = new Node(Guid1, NodeType.Cell);
            var r00 = tree.RegisterReproductionAndOffspring(new[] {Root}, node00);

            LayoutNodeTest.AssertDisplayHierarchy(@"
00
00
00",
                layoutNodes[Root.Guid]);

            Assert.AreEqual(3, layoutNodes.Count);
            var lr00 = layoutNodes[r00.Guid];
            Assert.AreSame(r00, lr00.Node);
            Assert.AreSame(Root, lr00.Parent.Node);
            var l00 = layoutNodes[node00.Guid];
            Assert.AreSame(node00, l00.Node);
            Assert.AreSame(r00, l00.Parent.Node);
        }

        [Test]
        public void TestTwoChildGenealogyLayout()
        {
            var tree = new GenealogyGraph();
            var layoutNodes = new Dictionary<Guid, LayoutNode>();
            var layout = new LayoutManager(layoutNodes);
            tree.AddListener(layout);

            tree.RegisterRootNode(Root);
            var node11 = new Node(Guid1, NodeType.Cell);
            var node12 = new Node(Guid2, NodeType.Cell);
            tree.RegisterReproductionAndOffspring(new[] {Root}, node11);
            tree.RegisterReproductionAndOffspring(new[] {Root}, node12);

            LayoutNodeTest.AssertDisplayHierarchy(@"
  00
00  01
00  00",
                layoutNodes[Root.Guid]);
        }

        [Test]
        public void ListenerAdditionTest()
        {
            var tree = new GenealogyGraph();
            var layoutNodes = new Dictionary<Guid, LayoutNode>();
            var layout = new LayoutManager(layoutNodes);
            tree.AddListener(layout);

            tree.RegisterRootNode(Root);
            Assert.AreEqual(1, layoutNodes.Count);

            tree.RemoveListener(layout);
            tree.RegisterReproductionAndOffspring(new[] {Root}, new Node(Guid1, NodeType.Reproduction));
            Assert.AreEqual(1, layoutNodes.Count);
        }
    }
}