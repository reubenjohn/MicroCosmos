using Genealogy.Graph;
using UnityEngine;

namespace Genealogy.Visualization
{
    public class ReproductionViewerNode : ViewerNode
    {
        public override Node GenealogyNode { get; set; }

        public static ReproductionViewerNode InstantiateNode(Transform parentTransform, Reproduction cellNode)
        {
            return InstantiateNode<ReproductionViewerNode>("UI/ReproductionViewerNode", parentTransform, cellNode);
        }
    }
}