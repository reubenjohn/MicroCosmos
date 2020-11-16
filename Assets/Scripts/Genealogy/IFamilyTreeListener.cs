using System.Collections.Generic;

namespace Genealogy
{
    public interface IFamilyTreeListener
    {
        void OnTransactionComplete(FamilyTree familyTree, Node node, List<Relation> relations);
    }
}