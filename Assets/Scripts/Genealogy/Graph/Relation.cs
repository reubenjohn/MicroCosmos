using System;
using Genealogy.Persistence;
using Newtonsoft.Json;

namespace Genealogy.Graph
{
    public class Relation
    {
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

        [NodeAsGuid] public Node From { get; }

        public RelationType RelationType { get; }
        [NodeAsGuid] public Node To { get; }
        public DateTime DateTime { get; }

        [JsonIgnore] public Tuple<Guid, Guid> Key => Tuple.Create(From.Guid, To.Guid);

        public override string ToString()
        {
            return $"({From.Guid}-{RelationType}->{To.Guid}@{DateTime})";
        }
    }
}