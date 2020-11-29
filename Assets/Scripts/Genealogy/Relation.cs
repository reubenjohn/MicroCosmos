using System;
using Newtonsoft.Json;

namespace Genealogy
{
    public class Relation
    {
        [NodeAsGuid] public Node From { get; }

        public RelationType RelationType { get; }
        [NodeAsGuid] public Node To { get; }
        public DateTime DateTime { get; }

        [JsonIgnore] public Tuple<Guid, Guid> Key => Tuple.Create(From.Guid, To.Guid);

        [JsonConstructor]
        public Relation(Node from, RelationType relationType, Node to, DateTime dateTime)
        {
            From = from;
            RelationType = relationType;
            To = to;
            DateTime = dateTime;
        }

        public Relation(Node from, RelationType relationType, Node to) :
            this(from, relationType, to, DateTime.Now)
        {
        }

        public override string ToString() => $"({From.Guid}-{RelationType}->{To.Guid}@{DateTime})";
    }
}