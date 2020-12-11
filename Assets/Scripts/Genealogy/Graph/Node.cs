using System;
using Genealogy.Persistence;
using Newtonsoft.Json;

namespace Genealogy.Graph
{
    public class Node
    {
        public Node(Guid guid, NodeType nodeType) : this(guid, nodeType, DateTime.Now)
        {
        }

        [JsonConstructor]
        public Node(Guid guid, NodeType nodeType, DateTime createdAt)
        {
            Guid = guid;
            NodeType = nodeType;
            CreatedAt = createdAt;
        }

        public NodeType NodeType { get; }

        [JsonConverter(typeof(GuidConverter))] public Guid Guid { get; }

        public DateTime CreatedAt { get; }

        public override string ToString() => $"{NodeType}@{CreatedAt}~{Guid}";
    }
}