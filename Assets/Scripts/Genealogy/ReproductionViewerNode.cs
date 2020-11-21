using UnityEngine;

namespace Genealogy
{
    public class ReproductionViewerNode : ViewerNode
    {
        public override Node GenealogyNode { get; set; }

        public static ReproductionViewerNode InstantiateNode(Transform parentTransform, Reproduction cellNode) =>
            InstantiateNode<ReproductionViewerNode>("UI/ReproductionViewerNode", parentTransform, cellNode);
    }
}