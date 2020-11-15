using System;
using UnityEngine;

namespace Genealogy
{
    public class CellNode : Node
    {
        private bool expanded = false;
        public int Generation { get; }
        public string Name { get; }
        public GameObject CellObj { get; set; }
        public GameObject CellViewerNode { get; set; }

        public CellNode() : this(null, null)
        {
        }

        public CellNode(GameObject cellObj, GameObject cellViewerNode)
            : this(Guid.NewGuid(), DateTime.Now, cellObj, cellViewerNode, 0)
        {
            CellObj = cellObj;
        }

        public CellNode(Guid guid, DateTime registrationTime,
            GameObject cellObj, GameObject cellViewerNode, int generation) :
            base(guid, NodeType.Cell, registrationTime)
        {
            CellObj = cellObj;
            CellViewerNode = cellViewerNode;
            Generation = generation;
        }
    }
}