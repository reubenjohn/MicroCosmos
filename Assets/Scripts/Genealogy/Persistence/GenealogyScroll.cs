using System;
using System.Collections.Generic;
using Genealogy.Graph;
using Newtonsoft.Json;

namespace Genealogy.Persistence
{
    public class GenealogyScroll
    {
        public readonly IEnumerable<GenealogyScrollEntry> entries;
        public readonly GenealogyScrollRootEntry rootEntry;

        public GenealogyScroll(GenealogyScrollRootEntry rootEntry, IEnumerable<GenealogyScrollEntry> entries)
        {
            this.rootEntry = rootEntry;
            this.entries = entries;
        }
    }

    public class GenealogyScrollRootEntry : IDisposable
    {
        [JsonConstructor]
        public GenealogyScrollRootEntry(Node rootNode)
        {
            RootNode = rootNode;
        }

        public Node RootNode { get; private set; }

        public void Dispose()
        {
            RootNode = null;
        }
    }

    public class GenealogyScrollEntry : IDisposable
    {
        public GenealogyScrollEntry(Node node, List<Relation> relations)
        {
            Node = node;
            Relations = relations;
        }

        public Node Node { get; private set; }
        public List<Relation> Relations { get; private set; }

        public void Dispose()
        {
            Node = null;
            Relations = null;
        }
    }
}