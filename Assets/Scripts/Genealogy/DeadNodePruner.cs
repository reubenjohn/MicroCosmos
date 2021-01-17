using System.Linq;
using Genealogy.Graph;
using UnityEngine.Assertions;

namespace Genealogy
{
    public class DeadNodePruner : IGenealogyGraphListener
    {
        public void OnAddTransactionComplete(GenealogyGraph genealogyGraph, Node node, Relation[] relations)
        {
            if (node.NodeType != NodeType.Death) return;
            Assert.AreEqual(1, relations.Length);
            PruneAncestry(genealogyGraph, relations[0]);
        }

        public void OnRemoveTransactionComplete(GenealogyGraph genealogyGraph, Node node, Relation[] relations) { }

        public void OnClear() { }

        private static void PruneAncestry(GenealogyGraph genealogyGraph, Relation deathRelation)
        {
            do
            {
                var cellNode = deathRelation.From;
                var cellEvents = genealogyGraph.GetRelationsFrom(cellNode.Guid);

                var hasLivingChildren = cellEvents.Any(r => r.RelationType == RelationType.Reproduction);
                if (hasLivingChildren) return;

                var deathNode = deathRelation.To;
                var reproductionNode = GetSingleParent(genealogyGraph, cellNode);
                var parentNode = GetSingleParent(genealogyGraph, reproductionNode);

                genealogyGraph.RemoveNodeAndRelations(deathNode);
                genealogyGraph.RemoveNodeAndRelations(cellNode);
                genealogyGraph.RemoveNodeAndRelations(reproductionNode);

                deathRelation = genealogyGraph.GetRelationsFrom(parentNode.Guid)
                    ?.Find(r => r.RelationType == RelationType.Death);
            } while (deathRelation != null);
        }

        private static Node GetSingleParent(GenealogyGraph genealogyGraph, Node child)
        {
            var parentRelations = genealogyGraph.GetRelationsTo(child.Guid);
            Assert.AreEqual(1, parentRelations.Count);
            return parentRelations[0].From;
        }
    }
}