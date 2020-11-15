using System;

namespace Genealogy
{
    public class Relation
    {
        public Node From { get; private set; }
        public RelationType RelationType { get; private set; }
        public Node To { get; private set; }
        public DateTime DateTime { get; }

        public Tuple<Guid, Guid> Key => Tuple.Create(From.Guid, To.Guid);

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