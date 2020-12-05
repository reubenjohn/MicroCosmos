﻿using UnityEngine;

namespace Genealogy
{
    public class CellDeathViewerNode : ViewerNode
    {
        public override Node GenealogyNode { get; set; }

        public static CellDeathViewerNode InstantiateNode(Transform parentTransform, CellDeath cellNode)
        {
            return InstantiateNode<CellDeathViewerNode>("UI/CellDeathViewerNode", parentTransform, cellNode);
        }
    }
}