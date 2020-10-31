using UnityEngine;

public class CellData
{
    public GeneNode geneTree;
    public StateNode stateTree;

    public static CellData Save(Cell cell) => new CellData()
    {
        geneTree = cell.SaveGeneTree(),
        stateTree = cell.SaveStateTree(),
    };

    public static void Load(CellData cellData, Transform container)
    {
        GameObject gameObject = LivingComponentUtils.LoadGeneTree(cellData.geneTree, container);
        gameObject.GetComponent<ILivingComponent>().LoadStateTree(cellData.stateTree);
    }
}
