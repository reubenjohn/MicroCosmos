using System;
using System.Collections.Generic;
using System.Linq;

namespace Genealogy.Graph
{
    public partial class GenealogyGraph
    {
        private readonly List<IGenealogyGraphListener> listeners;
        private readonly Dictionary<Guid, Node> nodes;
        private readonly Dictionary<Tuple<Guid, Guid>, Relation> relations;
        private readonly Dictionary<Guid, List<Relation>> relationsByFrom;
        private readonly Dictionary<Guid, List<Relation>> relationsByTo;

        private readonly Transaction transaction;

        private readonly Relation[] unitRelation = new Relation[1];
        public Node rootNode;

        public GenealogyGraph(Dictionary<Guid, Node> nodes, Dictionary<Tuple<Guid, Guid>, Relation> relations)
        {
            this.nodes = nodes;
            this.relations = relations;
            relationsByFrom = relations.GroupBy(pair => pair.Key.Item1, pair => pair.Value)
                .ToDictionary(group => group.Key, group => group.ToList());
            relationsByTo = relations.GroupBy(pair => pair.Key.Item2, pair => pair.Value)
                .ToDictionary(group => group.Key, group => group.ToList());

            transaction = new Transaction(this);
            listeners = new List<IGenealogyGraphListener>();
        }

        public GenealogyGraph() : this(
            new Dictionary<Guid, Node>(),
            new Dictionary<Tuple<Guid, Guid>, Relation>()) { }

        public int NodeCount => nodes.Count;

        public int RelationCount => relations.Count;

        public IEnumerable<Relation> Relations => relations.Values;

        public void Clear()
        {
            foreach (var listener in listeners) listener.OnClear();

            transaction.Reset();
            rootNode = null;
            nodes.Clear();
            relations.Clear();
            relationsByFrom.Clear();
            relationsByTo.Clear();
        }

        public Node GetNode(Guid guid) => nodes[guid];

        public Node GetNodeOrDefault(Guid guid) => nodes.TryGetValue(guid, out var node) ? node : default;

        public List<Relation> GetRelationsFrom(Guid guid) =>
            relationsByFrom.TryGetValue(guid, out var relation) ? relation : null;

        public List<Relation> GetRelationsTo(Guid guid) =>
            relationsByTo.TryGetValue(guid, out var relation) ? relation : null;

        public Relation GetRelation(Guid from, Guid to) => GetRelation(Tuple.Create(from, to));

        public Relation GetRelation(Tuple<Guid, Guid> fromTo) =>
            relations.TryGetValue(fromTo, out var relation) ? relation : null;

        public Reproduction RegisterReproductionAndOffspring(Node[] parents, Node child)
        {
            foreach (var name in parents.Select(node => node.Guid))
                if (!nodes.ContainsKey(name))
                    throw new InvalidOperationException(
                        $"Cannot register reproduction unless parents are themselves registered. Please first register parent: '{name}'.");
            if (nodes.TryGetValue(child.Guid, out var existingChild))
                throw new InvalidOperationException(
                    $"A reproduction for a given child can only be registered once. Child '{child.Guid}' was already first registered at {existingChild.CreatedAt}");

            var reproductionTime = new DateTime(child.CreatedAt.Ticks - 1);
            var reproduction = new Reproduction(Guid.NewGuid(), reproductionTime);

            transaction.ExecuteAddTransaction(reproduction, parents.Select(
                    parent => new Relation(parent, RelationType.Reproduction, reproduction, reproductionTime))
                .ToArray());

            unitRelation[0] = new Relation(reproduction, RelationType.Offspring, child, child.CreatedAt);
            transaction.ExecuteAddTransaction(child, unitRelation);

            return reproduction;
        }

        public Reproduction RegisterReproduction(Node[] parents, Reproduction reproduction)
        {
            foreach (var name in parents.Select(node => node.Guid))
                if (!nodes.ContainsKey(name))
                    throw new InvalidOperationException(
                        $"Cannot register reproduction unless parents are themselves registered. Please first register parent: '{name}'.");
            if (nodes.TryGetValue(reproduction.Guid, out var existingReproduction))
                throw new InvalidOperationException(
                    $"A reproduction can only be registered once. Reproduction '{reproduction.Guid}' was already first registered at {existingReproduction.CreatedAt}");

            transaction.ExecuteAddTransaction(reproduction, parents.Select(
                    parent => new Relation(parent, RelationType.Reproduction, reproduction, reproduction.CreatedAt))
                .ToArray());
            return reproduction;
        }

        public Reproduction RegisterOffspring(Reproduction reproduction, CellNode child)
        {
            if (!nodes.ContainsKey(reproduction.Guid))
                throw new InvalidOperationException(
                    $"Cannot register offspring until its reproduction is itself registered. Please first register reproduction: '{reproduction}'.");
            if (nodes.TryGetValue(child.Guid, out var existingChild))
                throw new InvalidOperationException(
                    $"A reproduction can only be registered once. Reproduction '{reproduction.Guid}' was already first registered at {existingChild.CreatedAt}");

            unitRelation[0] = new Relation(reproduction, RelationType.Offspring, child, child.CreatedAt);
            transaction.ExecuteAddTransaction(child, unitRelation);

            return reproduction;
        }

        public Reproduction RegisterReproductionAndOffspring(Node[] parents, CellNode child) =>
            RegisterReproductionAndOffspring(parents, (Node) child);

        public CellDeath RegisterDeath(CellNode cellNode, CellDeath cellDeath)
        {
            if (!nodes.ContainsKey(cellNode.Guid))
                throw new InvalidOperationException(
                    $"Cannot register death unless node is itself registered. Please first register parent: '{cellNode.Guid}'.");
            if (nodes.TryGetValue(cellDeath.Guid, out var existingDeath))
                throw new InvalidOperationException(
                    $"A death can only be registered once. CellDeath '{cellDeath.Guid}' was already first registered at {existingDeath.CreatedAt}");

            unitRelation[0] = new Relation(cellNode, RelationType.Death, cellDeath, cellDeath.CreatedAt);
            transaction.ExecuteAddTransaction(cellDeath, unitRelation);

            return cellDeath;
        }

        public Relation RegisterRelation(Relation relation)
        {
            unitRelation[0] = relation;
            transaction.ExecuteAddRelationTransaction(unitRelation);
            return relation;
        }

        public void AddListener(IGenealogyGraphListener listener) => listeners.Add(listener);

        public void RemoveListener(IGenealogyGraphListener listener) => listeners.Remove(listener);

        public void RegisterRootNode(Node root)
        {
            if (NodeCount > 0) throw new InvalidOperationException("The root node must be the first node registered");
            transaction.ExecuteRootNodeTransaction(root);
            rootNode = root;
        }
    }
}