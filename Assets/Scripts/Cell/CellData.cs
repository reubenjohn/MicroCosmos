using UnityEngine;

public class CellData
{
    public GeneNode geneTree;
    public StateNode stateTree;

    public static CellData Save(Cell cell) => new CellData()
    {
        geneTree = GeneNode.Save(cell),
        stateTree = StateNode.Save(cell),
    };

    public static void Load(CellData cellData, Transform container)
    {
        GameObject gameObject = GeneNode.Load(cellData.geneTree, container);
        StateNode.Load(cellData.geneTree.livingComponent, cellData.stateTree);
    }
}
