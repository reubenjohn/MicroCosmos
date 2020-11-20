using System;
using System.Collections.Generic;
using System.Linq;

namespace Genealogy
{
    public partial class FamilyTree
    {
        public Node rootNode;
        private readonly Dictionary<Guid, Node> nodes;
        private readonly Dictionary<Tuple<Guid, Guid>, Relation> relations;
        private readonly Dictionary<Guid, List<Relation>> relationsByFrom;
        private readonly Dictionary<Guid, List<Relation>> relationsByTo;

        private readonly Transaction transaction;

        private readonly List<IFamilyTreeListener> listeners;

        public FamilyTree(Dictionary<Guid, Node> nodes, Dictionary<Tuple<Guid, Guid>, Relation> relations)
        {
            this.nodes = nodes;
            this.relations = relations;
            relationsByFrom = relations.GroupBy(pair => pair.Key.Item1, pair => pair.Value)
                .ToDictionary(group => group.Key, group => group.ToList());
            relationsByTo = relations.GroupBy(pair => pair.Key.Item2, pair => pair.Value)
                .ToDictionary(group => group.Key, group => group.ToList());

            transaction = new Transaction(this);
            listeners = new List<IFamilyTreeListener>();
        }

        public FamilyTree() : this(
            new Dictionary<Guid, Node>(),
            new Dictionary<Tuple<Guid, Guid>, Relation>())
        {
        }

        public Node GetNode(Guid guid) => nodes[guid];

        public int NodeCount => nodes.Count;

        public List<Relation> GetRelationsFrom(Guid guid) =>
            relationsByFrom.TryGetValue(guid, out var relation) ? relation : null;

        public List<Relation> GetRelationsTo(Guid guid) =>
            relationsByTo.TryGetValue(guid, out var relation) ? relation : null;

        public Relation GetRelation(Guid from, Guid to) => GetRelation(Tuple.Create(from, to));

        public Relation GetRelation(Tuple<Guid, Guid> fromTo) =>
            relations.TryGetValue(fromTo, out var relation) ? relation : null;

        public int RelationCount => relations.Count;

        public Reproduction RegisterReproduction(Node[] parents, Node child, DateTime dateTime)
        {
            foreach (var name in parents.Select(node => node.Guid))
                if (!nodes.ContainsKey(name))
                    throw new InvalidOperationException(
                        $"Cannot register reproduction unless parents are themselves registered. " +
                        $"Please first register parent: '{name}'.");
            if (nodes.TryGetValue(child.Guid, out var existingChild))
                throw new InvalidOperationException(
                    "A reproduction for a given child can only be registered once. " +
                    $"Child '{child.Guid}' was already first registered at {existingChild.RegistrationTime}");

            var reproduction = new Reproduction(Guid.NewGuid(), dateTime);
            transaction.StartNew(reproduction);
            foreach (var parent in parents)
                transaction.AddRelation(new Relation(parent, RelationType.Reproduction, reproduction, dateTime));
            transaction.Complete();

            transaction.StartNew(child);
            transaction.AddRelation(new Relation(reproduction, RelationType.Offspring, child, dateTime));
            transaction.Complete();

            return reproduction;
        }

        public Reproduction RegisterReproduction(Node[] parents, Node child) =>
            RegisterReproduction(parents, child, DateTime.Now);

        public Relation RegisterRelation(Relation relation)
        {
            transaction.AddRelation(relation);
            transaction.Complete();
            return relation;
        }

        private Relation[] RegisterRelationsWithoutNotify(params Relation[] relationsToAdd)
        {
            foreach (var relation in relationsToAdd)
            {
                if (!nodes.ContainsKey(relation.From.Guid))
                    throw new InvalidOperationException(
                        "Participating nodes must first themselves be registered " +
                        "before a relationship between them can be registered. " +
                        $"Please first register the 'from' side node: '{relation.From.Guid}'");
                if (!nodes.ContainsKey(relation.To.Guid))
                    throw new InvalidOperationException(
                        "Participating nodes must first themselves be registered " +
                        "before a relationship between them can be registered. " +
                        $"Please first register the 'to' side node: '{relation.To.Guid}'");

                if (relations.TryGetValue(relation.Key, out var existingRelation))
                    throw new InvalidOperationException(
                        "There can only exist a single relation between two nodes. " +
                        $"Existing relation: '{existingRelation}'");
            }

            foreach (var relation in relationsToAdd)
            {
                relations.Add(relation.Key, relation);

                {
                    if (relationsByFrom.TryGetValue(relation.From.Guid, out var fromRelations))
                        fromRelations.Add(relation);
                    else
                        relationsByFrom.Add(relation.From.Guid, new List<Relation>() {relation});
                }

                {
                    if (relationsByTo.TryGetValue(relation.To.Guid, out var toRelations))
                        toRelations.Add(relation);
                    else
                        relationsByTo.Add(relation.To.Guid, new List<Relation>() {relation});
                }
            }

            return relationsToAdd;
        }

        public void AddListener(IFamilyTreeListener listener) => listeners.Add(listener);
        public void RemoveListener(IFamilyTreeListener listener) => listeners.Remove(listener);

        public void RegisterRootNode(Node root)
        {
            if (NodeCount > 0) throw new InvalidOperationException("The root node must be the first node registered");
            transaction.StartNew(root);
            transaction.Complete();
            rootNode = root;
        }
    }
}