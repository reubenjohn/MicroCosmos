using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Genealogy.Persistence
{
    public class GenealogyScroll
    {
        public readonly GenealogyScrollRootEntry rootEntry;

        public readonly IEnumerable<GenealogyScrollEntry> entries;

        public GenealogyScroll(GenealogyScrollRootEntry rootEntry, IEnumerable<GenealogyScrollEntry> entries)
        {
            this.rootEntry = rootEntry;
            this.entries = entries;
        }
    }

    public class GenealogyScrollRootEntry : IDisposable
    {
        public Node RootNode { get; private set; }

        [JsonConstructor]
        public GenealogyScrollRootEntry(Node rootNode)
        {
            RootNode = rootNode;
        }

        public void Dispose() => RootNode = null;
    }

    public class GenealogyScrollEntry : IDisposable
    {
        public Node Node { get; private set; }
        public List<Relation> Relations { get; private set; }

        public GenealogyScrollEntry(Node node, List<Relation> relations)
        {
            Node = node;
            Relations = relations;
        }

        public void Dispose()
        {
            Node = null;
            Relations = null;
        }
    }
}