using System;
using System.Collections.Generic;
using Genealogy;
using NUnit.Framework;

namespace Tests.EditMode.Genealogy
{
    public class FamilyTreeTest
    {
        private static readonly Node Root = new Node(Guid.Parse("00000000-0000-0000-0000-000000000000"), NodeType.Cell);

        private static readonly Guid Guid1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid Guid2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid Guid3 = Guid.Parse("33333333-3333-3333-3333-333333333333");

        [Test]
        public void GuidSanityCheck()
        {
            Assert.IsFalse(Guid1.Equals(null));
            Assert.IsTrue(Guid1.Equals(Guid1));
            Assert.IsFalse(Guid1.Equals(new object()));
            Assert.IsFalse(Guid1.Equals(Guid2));
        }

        [Test]
        public void NodeToString2()
        {
            var node = new Node(Guid1, NodeType.Cell, new DateTime(2020, 1, 1, 13, 1, 1));
            Assert.AreEqual($"Cell@1/1/2020 1:01:01 PM~{Guid1}", node.ToString());
        }

        [Test]
        public void CellNodeToString2()
        {
            var node = new Node(Guid1, NodeType.Cell, new DateTime(2020, 1, 1, 13, 1, 1));
            Assert.AreEqual($"Cell@1/1/2020 1:01:01 PM~{Guid1}", node.ToString());
        }

        [Test]
        public void RelationToString2()
        {
            var node1 = new Node(Guid1, NodeType.Cell, new DateTime(2020, 1, 1, 13, 1, 1));
            var node2 = new Node(Guid2, NodeType.Reproduction, new DateTime(2020, 1, 1, 13, 1, 1));
            var relation = new Relation(node1, RelationType.Offspring, node2, new DateTime(2020, 1, 1, 13, 1, 2));
            Assert.AreEqual($"({Guid1}-Offspring->{Guid2}@1/1/2020 1:01:02 PM)", relation.ToString());
        }

        [Test]
        public void TestRegisterRelation()
        {
            var nodes = new Dictionary<Guid, Node>();
            var relations = new Dictionary<Tuple<Guid, Guid>, Relation>();
            var tree = new FamilyTree(nodes, relations);

            var node1 = new Node(Guid1, NodeType.Cell, new DateTime(2020, 1, 1, 13, 1, 1));
            var node2 = new Node(Guid2, NodeType.Reproduction, new DateTime(2020, 1, 1, 13, 1, 1));

            var invalidOp =
                Assert.Throws<InvalidOperationException>(() =>
                    tree.RegisterRelation(new Relation(node1, RelationType.Reproduction, node2)));
            StringAssert.Contains($"Please first register the 'from' side node: '{node1.Guid}'", invalidOp.Message);
            Assert.AreEqual(0, nodes.Count);
            Assert.AreEqual(0, relations.Count);
            tree.AbortTransaction();

            nodes.Add(node1.Guid, node1);
            invalidOp = Assert.Throws<InvalidOperationException>(() =>
                tree.RegisterRelation(new Relation(node1, RelationType.Reproduction, node2)));
            StringAssert.Contains($"Please first register the 'to' side node: '{node2.Guid}'", invalidOp.Message);
            tree.AbortTransaction();
            Assert.AreEqual(1, nodes.Count);
            Assert.AreEqual(0, relations.Count);

            Assert.IsNull(tree.GetRelation(node1.Guid, node2.Guid));
            Assert.IsNull(tree.GetRelation(Tuple.Create(node1.Guid, node2.Guid)));
            Assert.IsNull(tree.GetRelationsFrom(node1.Guid));
            Assert.IsNull(tree.GetRelationsTo(node2.Guid));

            var now = DateTime.Now;
            nodes.Add(node2.Guid, node2);
            var relation = tree.RegisterRelation(new Relation(node1, RelationType.Reproduction, node2, now));
            Assert.AreEqual(new Relation(node1, RelationType.Reproduction, node2, now).ToString(), relation.ToString());

            Assert.AreEqual(relation.ToString(), tree.GetRelation(node1.Guid, node2.Guid).ToString());
            Assert.AreEqual(relation.ToString(), tree.GetRelation(Tuple.Create(node1.Guid, node2.Guid)).ToString());
            Assert.AreEqual(1, tree.GetRelationsFrom(node1.Guid).Count);
            Assert.AreEqual(1, tree.GetRelationsTo(node2.Guid).Count);
            Assert.AreEqual(relation.ToString(), tree.GetRelationsFrom(node1.Guid)[0].ToString());
            Assert.AreEqual(relation.ToString(), tree.GetRelationsTo(node2.Guid)[0].ToString());

            invalidOp = Assert.Throws<InvalidOperationException>(() =>
                tree.RegisterRelation(new Relation(node1, RelationType.Reproduction, node2)));
            StringAssert.Contains($"Existing relation: '{relation}'", invalidOp.Message);
        }

        [Test]
        public void TestRootNodeRegistration()
        {
            var tree = new FamilyTree();

            tree.RegisterRootNode(Root);

            Assert.AreEqual(1, tree.NodeCount);

            var ex = Assert.Throws<InvalidOperationException>(() =>
                tree.RegisterRootNode(new Node(Guid1, NodeType.Cell)));
            StringAssert.Contains("must be the first", ex.Message);
        }

        [Test]
        public void TestRegisterReproduction()
        {
            var tree = new FamilyTree();
            tree.RegisterRootNode(Root);

            var node1 = new Node(Guid1, NodeType.Cell);
            var node2 = new Node(Guid2, NodeType.Cell);
            var node3 = new Node(Guid3, NodeType.Cell);

            Assert.AreEqual(1, tree.NodeCount);
            Assert.AreEqual(0, tree.RelationCount);

            Assert.Throws<InvalidOperationException>(() => tree.RegisterReproduction(new[] {node1}, node2));
            var dateTime = new DateTime(2020, 1, 1, 13, 1, 1);
            var reproduction = tree.RegisterReproduction(new[] {Root}, node1,
                dateTime);

            Assert.AreEqual(dateTime, reproduction.RegistrationTime);

            Assert.AreEqual(3, tree.NodeCount);
            Assert.AreEqual(reproduction.Guid, tree.GetNode(reproduction.Guid)?.Guid);
            Assert.AreEqual(Guid1, tree.GetNode(Guid1)?.Guid);

            Assert.AreEqual(2, tree.RelationCount);
            Assert.NotNull(tree.GetRelation(Root.Guid, reproduction.Guid));
            Assert.NotNull(tree.GetRelation(reproduction.Guid, Guid1));


            tree.RegisterReproduction(new[] {Root}, node2);
            tree.RegisterReproduction(new[] {node1, node2}, node3);

            Assert.AreEqual(7, tree.NodeCount);
            Assert.AreEqual(7, tree.RelationCount);
        }
    }
}