using System;
using Genetics;
using Persistence;
using UnityEngine;

namespace Cell
{
    [Serializable]
    public class CellData
    {
        public GeneNode geneTree { get; set; }
        public StateNode stateTree { get; set; }

        public static CellData Save(Cell cell) => new CellData
        {
            geneTree = GeneNode.Save(cell),
            stateTree = StateNode.Save(cell)
        };

        public static GameObject Load(CellData cellData, Transform container)
        {
            var gameObject = GeneNode.Load(cellData.geneTree, container);
            StateNode.Load(gameObject.GetComponent<ILivingComponent>(), cellData.stateTree);
            return gameObject;
        }
    }
}