using System;
using UnityEngine;

namespace Genealogy
{
    public class CellNode : Node
    {
        public GameObject CellObj { get; }
        public GameObject CellViewerNode { get; }

        public readonly string displayName;

        public CellNode()
            : this(Guid.NewGuid(), DateTime.Now, null, null, "")
        {
        }

        public CellNode(GameObject cellObj, string displayName)
            : this(Guid.NewGuid(), DateTime.Now, cellObj, null, displayName)
        {
        }

        public CellNode(Guid guid, DateTime registrationTime,
            GameObject cellObj, GameObject cellViewerNode, string displayName) :
            base(guid, NodeType.Cell, registrationTime)
        {
            CellObj = cellObj;
            CellViewerNode = cellViewerNode;
            this.displayName = displayName;
        }

        public override string ToString() => displayName;
    }
}