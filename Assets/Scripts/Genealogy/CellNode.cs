using System;
using Newtonsoft.Json;

namespace Genealogy
{
    public class CellNode : Node
    {
        public readonly string displayName;

        public CellNode()
            : this(Guid.NewGuid(), DateTime.Now, null)
        {
        }

        public CellNode(string displayName)
            : this(Guid.NewGuid(), DateTime.Now, displayName)
        {
        }

        [JsonConstructor]
        public CellNode(Guid guid, DateTime registrationTime, string displayName) :
            base(guid, NodeType.Cell, registrationTime)
        {
            this.displayName = displayName;
        }

        public override string ToString() => displayName ?? base.ToString();
    }
}