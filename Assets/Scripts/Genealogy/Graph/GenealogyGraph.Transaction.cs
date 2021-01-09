using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Genealogy.Graph
{
    public partial class GenealogyGraph
    {
        public void AbortTransaction()
        {
            transaction.Abort();
        }

        private class Transaction
        {
            private readonly GenealogyGraph genealogyGraph;

            private DateTime lastTransactionTime = DateTime.MinValue;

            private Node node;

            public Transaction(GenealogyGraph genealogyGraph)
            {
                this.genealogyGraph = genealogyGraph;
                relations = new List<Relation>();
            }

            private List<Relation> relations { get; }

            public void StartNew([NotNull] Node transactionNode)
            {
                if (DateTime.Compare(transactionNode.CreatedAt, lastTransactionTime) <= 0)
                    throw new InvalidOperationException(
                        $"Cannot start a transaction with a node who's CreatedAt ({transactionNode.CreatedAt}) <= lastTransactionTime ({lastTransactionTime})");
                if (node != null)
                    throw new InvalidOperationException(
                        $"Cannot start new transaction until existing transaction for node {node} is complete");
                if (relations.Count > 0)
                    throw new InvalidOperationException(
                        $"Cannot start new transaction until existing transaction of {relations.Count} relations including '{relations[0]}' is complete");

                node = transactionNode;
            }

            public void AddRelation(Relation relation)
            {
                if (node != null)
                {
                    if (relation.To.Guid != node.Guid)
                        throw new InvalidOperationException(
                            $"Cannot add relations to node '{relation.To}' while in a transaction for node '{node}'");
                    if (!DateTime.Equals(relation.DateTime, node.CreatedAt))
                        throw new InvalidOperationException(
                            $"Cannot add relation to transaction. Relation's creation time '{relation.DateTime}' must equal transaction node's creation time {node.CreatedAt}");
                }
                else if (relations.Count > 0)
                {
                    throw new InvalidOperationException(
                        $"Cannot add relation. Non node transactions can only consist of a single relation. Current transaction already contains relation '{relations[0]}' and thus relation '{relation}' cannot be added");
                }

                relations.Add(relation);
            }

            public void Complete()
            {
                if (relations.Count == 0 && genealogyGraph.NodeCount != 0)
                    throw new InvalidOperationException(
                        "A transaction can only be completed without a relation if it is that of the root node");

                if (node != null)
                {
                    if (genealogyGraph.nodes.ContainsKey(node.Guid))
                        throw new InvalidOperationException(
                            $"Cannot complete transaction. Node {node} already exists in the tree");
                    genealogyGraph.nodes.Add(node.Guid, node);
                }

                genealogyGraph.RegisterRelationsWithoutNotify(relations.ToArray());

                foreach (var listener in genealogyGraph.listeners)
                    listener.OnTransactionComplete(genealogyGraph, node, relations);

                node = null;
                relations.Clear();
            }

            public void Abort()
            {
                node = null;
                relations.Clear();
            }

            public void Reset()
            {
                Abort();
                lastTransactionTime = DateTime.MinValue;
            }
        }
    }
}