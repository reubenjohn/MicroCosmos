using System;

namespace Genealogy
{
    public class Node
    {
        public Guid Guid { get; }
        public NodeType NodeType { get; }
        public DateTime RegistrationTime { get; }

        public Node(Guid guid, NodeType nodeType) : this(guid, nodeType, DateTime.Now)
        {
        }

        public Node(Guid guid, NodeType nodeType, DateTime registrationTime)
        {
            Guid = guid;
            NodeType = nodeType;
            RegistrationTime = registrationTime;
        }

        public override string ToString() => $"{NodeType}@{RegistrationTime}~{Guid}";
    }
}