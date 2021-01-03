using System;
using System.Collections.Generic;
using Genealogy.Graph;
using Genealogy.Layout;
using Genealogy.Layout.Asexual;
using Newtonsoft.Json;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace Tests.EditMode.Genealogy.Asexual
{
    public class LayoutNodeTest
    {
        private static readonly Node Root = new Node(Guid.Parse("00000000-0000-0000-0000-000000000000"), NodeType.Cell,
            new DateTime(2020, 1, 1));

        private static readonly Guid Guid1 = Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e");
        private static readonly Guid Guid2 = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7");
        private static readonly Guid Guid3 = Guid.Parse("e07fc1f9-d9cb-40de-a165-70867728950e");

        private TestLayoutChangeListener listener;
        private List<ILayoutChangeListener<LayoutNode>> listeners;

        [SetUp]
        public void Setup()
        {
            listener = new TestLayoutChangeListener();
            listeners = new List<ILayoutChangeListener<LayoutNode>> {listener};
        }

        private LayoutNode NewLayoutNode(Node node, LayoutNode parent) => new LayoutNode(listeners, node, parent);

        internal static void AssertDisplayHierarchy(string expected, LayoutNode node)
        {
            node.RecalculateLayout();
            var actual = node.GetHierarchyDisplayString(System.Environment.NewLine, 2);
            try
            {
                Assert.AreEqual(expected, actual);
            }
            catch (AssertionException e)
            {
                throw new AssertionException($@"
>Expected:{expected}
>But was:{actual}
Assertion message:
{e.Message}");
            }
        }

        [Test]
        public void TestGeneration()
        {
            var l0 = NewLayoutNode(Root, null);
            Assert.AreEqual(0, l0.Generation);
            var l00 = NewLayoutNode(Root, l0);
            Assert.AreEqual(1, l00.Generation);
        }

        [Test]
        public void TestListenerCallback()
        {
            AssertListenerCounts(0, 0, 0, 0);

            var l0 = NewLayoutNode(Root, null);
            AssertListenerCounts(1, 0, 0, 0);

            l0.RecalculateLayout();
            AssertListenerCounts(1, 1, 0, 0);

            var l00 = NewLayoutNode(Root, l0);
            AssertListenerCounts(2, 1, 0, 0);
            l0.RecalculateLayout();
            AssertListenerCounts(2, 3, 0, 0);
        }

        private void AssertListenerCounts(int addedNodeCount, int updatedNodeCount, int addedRelationsCount,
            int nClears)
        {
            Assert.AreEqual(addedNodeCount, listener.addedNodes.Count, "addedNodeCount");
            Assert.AreEqual(updatedNodeCount, listener.updatedNodes.Count, "updatedNodeCount");
            Assert.AreEqual(addedRelationsCount, listener.addedRelations.Count, "addedRelationsCount");
            Assert.AreEqual(nClears, listener.NClears);
        }

        [Test]
        public void TestRootNode()
        {
            var l0 = NewLayoutNode(Root, null);
            l0.RecalculateLayout();

            Assert.AreEqual(
                @"{""Node"":{""NodeType"":0,""Guid"":""00000000-0000-0000-0000-000000000000"",""CreatedAt"":""2020-01-01T00:00:00""},""Parent"":null,""Generation"":0,""SiblingIndex"":0,""Center"":{""x"":0.5,""y"":-0.5}}",
                JsonConvert.SerializeObject(l0));
            Assert.AreSame(Root, l0.Node);

            AssertDisplayHierarchy(@"
00",
                l0);
        }

        [Test]
        public void TestLinearLayout2Generation()
        {
            var l0 = NewLayoutNode(Root, null);
            var l00 = NewLayoutNode(new Node(Guid1, NodeType.Cell), l0);

            Assert.AreSame(l0, l00.Parent);
            Assert.AreEqual(0, l0.Generation);

            AssertDisplayHierarchy(@"
00
00",
                l0);
        }

        [Test]
        public void TestLinearLayout3Generation()
        {
            var l0 = NewLayoutNode(Root, null);
            var l00 = NewLayoutNode(new Node(Guid1, NodeType.Cell), l0);
            var l000 = NewLayoutNode(new Node(Guid2, NodeType.Cell), l00);

            AssertDisplayHierarchy(@"
00
00
00",
                l0);
        }

        [Test]
        public void Test2ChildLayout()
        {
            var l0 = NewLayoutNode(Root, null);
            var l00 = NewLayoutNode(new Node(Guid1, NodeType.Cell), l0);
            var l01 = NewLayoutNode(new Node(Guid2, NodeType.Cell), l0);

            AssertDisplayHierarchy(@"
  00
00  01",
                l0);
        }

        [Test]
        public void TestImpactedRightNode()
        {
            var l0 = NewLayoutNode(Root, null);
            var l00 = NewLayoutNode(null, l0);
            var l01 = NewLayoutNode(null, l0);
            var l001 = NewLayoutNode(null, l00);

            // Not impacted yet
            AssertDisplayHierarchy(@"
  00
00  01
00",
                l0);

            var l002 = NewLayoutNode(null, l00);
            AssertDisplayHierarchy(@"
    00
  00    01
00  01",
                l0);
        }

        [Test]
        public void TestImpactedRightTree()
        {
            var l0 = NewLayoutNode(Root, null);
            var l00 = NewLayoutNode(null, l0);
            var l01 = NewLayoutNode(null, l0);
            var l010 = NewLayoutNode(null, l01);
            var l011 = NewLayoutNode(null, l01);

            AssertDisplayHierarchy(@"
    00
00    01
    00  01",
                l0);

            var l000 = NewLayoutNode(null, l00);
            AssertDisplayHierarchy(@"
    00
00    01
00  00  01",
                l0);

            var l001 = NewLayoutNode(null, l00);

            AssertDisplayHierarchy(@"
      00
  00      01
00  01  00  01",
                l0);
        }

        [Test]
        public void TestCascadedTreeUpdate()
        {
            var l0 = NewLayoutNode(Root, null);
            var l00 = NewLayoutNode(null, l0);
            var l01 = NewLayoutNode(null, l0);
            var l010 = NewLayoutNode(null, l01);
            var l011 = NewLayoutNode(null, l01);
            var l000 = NewLayoutNode(null, l00);
            var l001 = NewLayoutNode(null, l00);
            var l0010 = NewLayoutNode(null, l001);

            AssertDisplayHierarchy(@"
      00
  00      01
00  01  00  01
    00",
                l0);

            var l0011 = NewLayoutNode(null, l001);
            AssertDisplayHierarchy(@"
        00
    00        01
00    01    00  01
    00  01",
                l0);

            var l00100 = NewLayoutNode(null, l0010);
            var l00101 = NewLayoutNode(null, l0010);
            AssertDisplayHierarchy(@"
          00
      00          01
00      01      00  01
      00    01
    00  01",
                l0);
        }

        [Test]
        public void TestCascadedTreeUpdate2()
        {
            var l0 = NewLayoutNode(Root, null);
            var l00 = NewLayoutNode(null, l0);
            var l01 = NewLayoutNode(null, l0);
            var l02 = NewLayoutNode(null, l0);
            var l020 = NewLayoutNode(null, l02);
            var l021 = NewLayoutNode(null, l02);
            var l010 = NewLayoutNode(null, l01);
            var l011 = NewLayoutNode(null, l01);

            AssertDisplayHierarchy(@"
        00
00    01      02
    00  01  00  01",
                l0);

            var l000 = NewLayoutNode(null, l00);
            var l001 = NewLayoutNode(null, l00);
            AssertDisplayHierarchy(@"
          00
  00      01      02
00  01  00  01  00  01",
                l0);
        }

        private class TestLayoutChangeListener : ILayoutChangeListener<LayoutNode>
        {
            public readonly List<LayoutNode> addedNodes = new List<LayoutNode>();
            public readonly List<List<Relation>> addedRelations = new List<List<Relation>>();
            public readonly List<LayoutNode> updatedNodes = new List<LayoutNode>();
            public int NClears { get; private set; }

            public void OnUpdateNode(LayoutNode layout) => updatedNodes.Add(layout);

            public void OnAddNode(LayoutNode layout) => addedNodes.Add(layout);

            public void OnAddConnections(List<Relation> relations) => addedRelations.Add(relations);

            public void OnClear() => NClears = NClears + 1;
        }
    }
}