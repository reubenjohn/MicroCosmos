using System.Collections.Generic;
using System.Linq;
using Genealogy.Graph;
using Newtonsoft.Json;
using UnityEngine.Assertions;

namespace Genealogy.Persistence
{
    public class ScrollReader
    {
        private readonly GenealogyGraph graph;

        public ScrollReader(GenealogyGraph graph)
        {
            this.graph = graph;
        }

        public static void Load(JsonReader sw, GenealogyGraph graph)
        {
            var jsonSerializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = new GenealogyScrollContractResolver(graph)
            };
            jsonSerializer.Deserialize<GenealogyScroll>(sw);
        }

        public void Load(IEnumerable<GenealogyScrollEntryBase> save)
        {
            graph.Clear();
            using (var entries = save.GetEnumerator())
            {
                if (entries.MoveNext())
                    using (entries.Current)
                    {
                        OnRootEntry((GenealogyScrollRootEntry) entries.Current);
                    }

                while (entries.MoveNext())
                    using (entries.Current)
                    {
                        OnEntry((GenealogyScrollEntry) entries.Current);
                    }
            }
        }

        private void OnRootEntry(GenealogyScrollRootEntry entry) => graph.RegisterRootNode(entry.Node);

        private void OnEntry(GenealogyScrollEntry entry)
        {
            var node = entry.Node;
            var relations = entry.Relations.Select(relation => new Relation(
                graph.GetNode(relation.From.Guid),
                relation.RelationType,
                node ?? graph.GetNode(relation.To.Guid)
            )).ToArray();
            if (node != null)
            {
                switch (node.NodeType)
                {
                    case NodeType.Reproduction:
                        var parents = relations.Select(r => r.From).ToArray();
                        graph.RegisterReproduction(parents, (Reproduction) node);
                        break;
                    case NodeType.Cell:
                        Assert.AreEqual(1, relations.Length);
                        var reproduction = relations[0].From;
                        graph.RegisterOffspring((Reproduction) reproduction, (CellNode) node);
                        break;
                }
            }
            else
            {
                Assert.AreEqual(1, relations.Length);
                graph.RegisterRelation(entry.Relations[0]);
            }
        }
    }
}