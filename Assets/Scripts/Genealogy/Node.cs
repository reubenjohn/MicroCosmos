using System;
using Genealogy.Persistence;
using Newtonsoft.Json;

namespace Genealogy
{
    public class Node
    {
        public NodeType NodeType { get; }
        
        [JsonConverter(typeof(GuidConverter))] public Guid Guid { get; }

        //TODO Rename to CreatedAt
        public DateTime RegistrationTime { get; }

        public Node(Guid guid, NodeType nodeType) : this(guid, nodeType, DateTime.Now)
        {
        }

        [JsonConstructor]
        public Node(Guid guid, NodeType nodeType, DateTime registrationTime)
        {
            Guid = guid;
            NodeType = nodeType;
            RegistrationTime = registrationTime;
        }

        public override string ToString() => $"{NodeType}@{RegistrationTime}~{Guid}";
    }
}