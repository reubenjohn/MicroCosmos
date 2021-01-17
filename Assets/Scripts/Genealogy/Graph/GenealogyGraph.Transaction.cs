using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Genealogy.Graph
{
    public partial class GenealogyGraph
    {
        private class Transaction
        {
            private static readonly Relation[] EmptyRelations = new Relation[0];
            private readonly GenealogyGraph genealogyGraph;

            private DateTime lastTransactionTime = DateTime.MinValue;

            public Transaction(GenealogyGraph genealogyGraph)
            {
                this.genealogyGraph = genealogyGraph;
            }

            public void ExecuteRootNodeTransaction([NotNull] Node node)
            {
                if (genealogyGraph.NodeCount != 0)
                    throw new InvalidOperationException(
                        $"Cannot register root node, the graph already has {genealogyGraph.NodeCount} nodes");
                genealogyGraph.nodes.Add(node.Guid, node);

                CompleteTransactionAndNotifyListeners(node, EmptyRelations);
            }

            public void ExecuteAddTransaction([NotNull] Node node,
                [CanBeNull] Relation[] toRelations)
            {
                if (DateTime.Compare(node.CreatedAt, lastTransactionTime) <= 0)
                    throw new InvalidOperationException(
                        $"Cannot start a transaction with a node '{node.Guid}' who's CreatedAt ({node.CreatedAt}) <= lastTransactionTime ({lastTransactionTime})");
                if (genealogyGraph.nodes.ContainsKey(node.Guid))
                    throw new InvalidOperationException(
                        $"Cannot complete transaction. Node {node} already exists in the tree");

                if (toRelations != null)
                    AssertRelations(toRelations, node);

                genealogyGraph.nodes.Add(node.Guid, node);

                if (toRelations != null)
                    foreach (var relation in toRelations)
                    {
                        genealogyGraph.relations.Add(relation.Key, relation);

                        {
                            if (genealogyGraph.relationsByFrom.TryGetValue(relation.From.Guid,
                                out var priorFromRelations))
                                priorFromRelations.Add(relation);
                            else
                                genealogyGraph.relationsByFrom.Add(relation.From.Guid, new List<Relation> {relation});
                        }
                        {
                            if (genealogyGraph.relationsByTo.TryGetValue(relation.To.Guid, out var priorToRelations))
                                priorToRelations.Add(relation);
                            else
                                genealogyGraph.relationsByTo.Add(relation.To.Guid, new List<Relation> {relation});
                        }
                    }

                CompleteTransactionAndNotifyListeners(node, toRelations);
            }

            private void AssertRelations([NotNull] IEnumerable<Relation> relations, [CanBeNull] Node toNode)
            {
                //TODO Check chronology
                var checkToNode = toNode != null;
                foreach (var relation in relations)
                {
                    if (!genealogyGraph.nodes.ContainsKey(relation.From.Guid))
                        throw new InvalidOperationException(
                            $"Participating nodes must first themselves be registered before a relationship between them can be registered. Please first register the 'from' side node: '{relation.From.Guid}'");
                    if (checkToNode
                        ? relation.To.Guid != toNode.Guid
                        : !genealogyGraph.nodes.ContainsKey(relation.To.Guid))
                        throw new InvalidOperationException(
                            $"Participating nodes must first themselves be registered before a relationship between them can be registered. Please first register the 'to' side node: '{relation.To.Guid}'");

                    if (genealogyGraph.relations.TryGetValue(relation.Key, out var existingRelation))
                        throw new InvalidOperationException(
                            $"There can only exist a single relation between two nodes. Existing relation: '{existingRelation}'");


                    if (checkToNode)
                    {
                        if (!DateTime.Equals(relation.DateTime, toNode.CreatedAt))
                            throw new InvalidOperationException(
                                $"Cannot add relation to transaction. Relation's creation time '{relation.DateTime}' must equal transaction node's creation time {toNode.CreatedAt}");
                    }
                    else if (relation.DateTime < lastTransactionTime)
                    {
                        throw new InvalidOperationException(
                            $"Cannot add relation to transaction. Relation's creation time '{relation.DateTime}' must be greater than or equal to that of the last transaction {lastTransactionTime}");
                    }
                }
            }

            public void ExecuteAddRelationTransaction([NotNull] Relation[] relations)
            {
                AssertRelations(relations, null);

                foreach (var relation in relations)
                {
                    genealogyGraph.relations.Add(relation.Key, relation);

                    {
                        if (genealogyGraph.relationsByFrom.TryGetValue(relation.From.Guid, out var priorFromRelations))
                            priorFromRelations.Add(relation);
                        else
                            genealogyGraph.relationsByFrom.Add(relation.From.Guid, new List<Relation> {relation});
                    }
                    {
                        if (genealogyGraph.relationsByTo.TryGetValue(relation.To.Guid, out var priorToRelations))
                            priorToRelations.Add(relation);
                        else
                            genealogyGraph.relationsByTo.Add(relation.To.Guid, new List<Relation> {relation});
                    }

                    CompleteTransactionAndNotifyListeners(null, relations);
                }
            }

            public void ExecuteRemoveNodeAndRelations([NotNull] Node transactionNode)
            {
                var guid = transactionNode.Guid;
                var relationsFrom = genealogyGraph.GetRelationsFrom(guid);
                if (relationsFrom != null && relationsFrom.Any())
                    throw new InvalidOperationException(
                        $"Child nodes must be removed before removal of node '{transactionNode.Guid}'");
                if (!genealogyGraph.nodes.ContainsKey(guid))
                    throw new InvalidOperationException(
                        $"Could not remove node '{guid}' as it does not exist in the graph");

                genealogyGraph.nodes.Remove(guid);
                foreach (var relation in genealogyGraph.GetRelationsTo(guid))
                {
                    var from = genealogyGraph.GetRelationsFrom(relation.From.Guid);
                    from.Remove(relation);
                    if (from.Count == 0) genealogyGraph.relationsByFrom.Remove(relation.From.Guid);
                    genealogyGraph.relationsByTo.Remove(relation.To.Guid);
                    genealogyGraph.relations.Remove(relation.Key);
                }
            }

            private void CompleteTransactionAndNotifyListeners([CanBeNull] Node node, Relation[] relations)
            {
                lastTransactionTime = node?.CreatedAt ?? relations.Select(relation => relation.DateTime).Max();
                foreach (var listener in genealogyGraph.listeners)
                    listener.OnTransactionComplete(genealogyGraph, node, relations);
            }

            public void Reset() => lastTransactionTime = DateTime.MinValue;
        }
    }
}