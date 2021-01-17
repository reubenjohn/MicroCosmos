namespace Genealogy.Graph
{
    public interface IGenealogyGraphListener
    {
        void OnTransactionComplete(GenealogyGraph genealogyGraph, Node node, Relation[] relations);
        void OnClear();
    }
}