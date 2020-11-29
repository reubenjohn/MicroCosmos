using System.Collections.Generic;

namespace Genealogy
{
    public interface IGenealogyGraphListener
    {
        void OnTransactionComplete(GenealogyGraph genealogyGraph, Node node, List<Relation> relations);
        void OnClear();
    }
}