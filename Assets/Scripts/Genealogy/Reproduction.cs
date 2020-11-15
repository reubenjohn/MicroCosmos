using System;

namespace Genealogy
{
    public class Reproduction : Node
    {
        public Reproduction(Guid guid) : this(guid, DateTime.Now)
        {
        }

        public Reproduction(Guid guid, DateTime dateTime) : base(guid, NodeType.Reproduction, dateTime)
        {
        }
    }
}