using System;
using System.Collections.Generic;
using Genealogy.Graph;
using Newtonsoft.Json;

namespace Genealogy.Persistence
{
    public abstract class GenealogyScroll
    {
        private readonly IEnumerable<GenealogyScrollEntry> entries;
        private readonly GenealogyScrollRootEntry rootEntry;

        [JsonConstructor]
        protected GenealogyScroll(GenealogyScrollRootEntry rootEntry, IEnumerable<GenealogyScrollEntry> entries)
        {
            this.rootEntry = rootEntry;
            this.entries = entries;
        }
    }

    public abstract class GenealogyScrollEntryBase : IDisposable
    {
        protected GenealogyScrollEntryBase(Node node)
        {
            Node = node;
        }

        [JsonProperty(Order = -2)] public Node Node { get; private set; }

        public virtual void Dispose() => Node = null;
    }

    public class GenealogyScrollRootEntry : GenealogyScrollEntryBase
    {
        [JsonConstructor]
        public GenealogyScrollRootEntry(Node node) : base(node) { }
    }

    public class GenealogyScrollEntry : GenealogyScrollEntryBase
    {
        [JsonConstructor]
        public GenealogyScrollEntry(Node node, Relation[] relations) : base(node)
        {
            Relations = relations;
        }

        public Relation[] Relations { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            Relations = null;
        }
    }
}