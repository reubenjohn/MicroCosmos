using System;
using System.Linq;
using System.Runtime.Serialization;
using Genealogy.Graph;
using Newtonsoft.Json.Serialization;
using UnityEngine.Assertions;

namespace Genealogy.Persistence
{
    public class GenealogyScrollContractResolver : NodeAsGuidContract
    {
        private readonly GenealogyGraph graph;

        public GenealogyScrollContractResolver(GenealogyGraph graph)
        {
            this.graph = graph;
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);

            var deserializedCallbacks = contract.OnDeserializedCallbacks;
            if (objectType == typeof(GenealogyScrollEntry))
            {
                // contract.DefaultCreator = () => new GenealogyScrollEntry(); TODO Recycle entry instance
                if (!deserializedCallbacks.Contains(OnEntry)) deserializedCallbacks.Add(OnEntry);
            }
            else if (objectType == typeof(GenealogyScrollRootEntry))
            {
                if (!deserializedCallbacks.Contains(OnRootEntry)) deserializedCallbacks.Add(OnRootEntry);
            }
            // TODO Prevent loading to list
            // else if (typeof(IEnumerable<GenealogyScrollEntry>).IsAssignableFrom(objectType))
            // {
            // }

            return contract;
        }

        private void OnRootEntry(object o, StreamingContext context)
        {
            var entry = (GenealogyScrollRootEntry) o;
            graph.RegisterRootNode(entry.RootNode);
            entry.Dispose();
        }

        private void OnEntry(object o, StreamingContext context)
        {
            var entry = (GenealogyScrollEntry) o;
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

            entry.Dispose();
        }
    }
}