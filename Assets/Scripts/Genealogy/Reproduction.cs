using System;
using Newtonsoft.Json;

namespace Genealogy
{
    public class Reproduction : Node
    {
        public Reproduction(Guid guid) : this(guid, DateTime.Now)
        {
        }

        [JsonConstructor]
        public Reproduction(Guid guid, DateTime registrationTime) : base(guid, NodeType.Reproduction, registrationTime)
        {
        }
    }
}