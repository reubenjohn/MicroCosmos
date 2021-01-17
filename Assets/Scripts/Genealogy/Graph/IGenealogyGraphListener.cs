namespace Genealogy.Graph
{
    public interface IGenealogyGraphListener
    {
        void OnAddTransactionComplete(GenealogyGraph genealogyGraph, Node node, Relation[] relations);
        void OnRemoveTransactionComplete(GenealogyGraph genealogyGraph, Node node, Relation[] relations);
        void OnClear();
    }
}