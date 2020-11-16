using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Genealogy
{
    public partial class FamilyTree
    {
        private class Transaction
        {
            private readonly FamilyTree familyTree;

            private Node node;
            private List<Relation> relations { get; }

            public Transaction(FamilyTree familyTree)
            {
                this.familyTree = familyTree;
                relations = new List<Relation>();
            }

            public void StartNew([NotNull] Node transactionNode)
            {
                if (node != null)
                    throw new InvalidOperationException($"Cannot start new transaction until " +
                                                        $"existing transaction for node {node} is complete");
                if (relations.Count > 0)
                    throw new InvalidOperationException(
                        "Cannot start new transaction until " +
                        $"existing transaction of {relations.Count} relations including '{relations[0]}' is complete");

                node = transactionNode;
            }

            public void AddRelation(Relation relation)
            {
                if (node != null)
                {
                    if (relation.To.Guid != node.Guid)
                        throw new InvalidOperationException($"Cannot add relations to node '{relation.To}' " +
                                                            $"while in a transaction for node '{node}'");
                }
                else if (relations.Count > 0)
                    throw new InvalidOperationException(
                        $"Cannot add relation. Non node transactions can only consist of a single relation. " +
                        $"Current transaction already contains relation '{relations[0]}' " +
                        $"and thus relation '{relation}' cannot be added");

                relations.Add(relation);
            }

            public void Complete()
            {
                if (relations.Count == 0 && familyTree.NodeCount != 0)
                    throw new InvalidOperationException(
                        "A transaction can only be completed without a relation if it is that of the root node");

                if (node != null)
                {
                    if (familyTree.nodes.ContainsKey(node.Guid))
                        throw new InvalidOperationException("Cannot complete transaction. " +
                                                            $"Node {node} already exists in the tree");
                    familyTree.nodes.Add(node.Guid, node);
                }

                familyTree.RegisterRelationsWithoutNotify(relations.ToArray());

                foreach (var listener in familyTree.listeners)
                    listener.OnTransactionComplete(familyTree, node, relations);

                node = null;
                relations.Clear();
            }

            public void Abort()
            {
                node = null;
                relations.Clear();
            }
        }
        
        public void AbortTransaction() => transaction.Abort();
    }
}