using System;
using Newtonsoft.Json;

namespace Genealogy.Graph
{
    public class CellDeath : Node
    {
        public CellDeath(Guid guid) : this(guid, DateTime.Now)
        {
        }

        [JsonConstructor]
        public CellDeath(Guid guid, DateTime createdAt) : base(guid, NodeType.Death, createdAt)
        {
        }
    }
}