using System;

namespace Genealogy
{
    public class Node
    {
        public Guid Guid { get; private set; }
        public NodeType NodeType { get; private set; }
        public DateTime RegistrationTime { get; private set; }

        public Node(Guid guid, NodeType nodeType) : this(guid, nodeType, DateTime.Now)
        {
        }

        public Node(Guid guid, NodeType nodeType, DateTime registrationTime)
        {
            Guid = guid;
            NodeType = nodeType;
            RegistrationTime = registrationTime;
        }

        public override string ToString() => $"{NodeType}~{Guid}@{RegistrationTime}";
    }
}