using System;
using System.Collections.Generic;
using Genealogy;
using Genealogy.Asexual;
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
        private static readonly List<ILayoutChangeListener<LayoutNode>> Listeners = new List<ILayoutChangeListener<LayoutNode>>();

        private LayoutNode NewLayoutNode(Node root, LayoutNode parent) => new LayoutNode(Listeners, root, parent);

        internal static void AssertDisplayHierarchy(string expected, LayoutNode node)
        {
            var actual = node.GetHierarchyDisplayString(nodeWidth:2);
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
        public void TestRootNode()
        {
            var l0 = NewLayoutNode(Root, null);

            Assert.AreEqual(
                @"{""Node"":{""NodeType"":0,""Guid"":""00000000-0000-0000-0000-000000000000"",""RegistrationTime"":""2020-01-01T00:00:00""},""Parent"":null,""Generation"":0,""XMax"":1.0,""SiblingIndex"":0,""X"":0.0,""Center"":{""x"":0.5,""y"":0.5}}",
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
    }
}