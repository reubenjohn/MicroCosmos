using System;
using System.Collections.Generic;

namespace Genealogy.AsexualFamilyTree
{
    public class LayoutManager : IFamilyTreeListener
    {
        private readonly Dictionary<Guid, LayoutNode> layoutInfo;

        public LayoutManager() : this(new Dictionary<Guid, LayoutNode>())
        {
        }

        public LayoutManager(Dictionary<Guid, LayoutNode> layoutInfo)
        {
            this.layoutInfo = layoutInfo;
        }

        public void OnTransactionComplete(FamilyTree familyTree, Node node, List<Relation> relations)
        {
            if (relations.Count == 0) // Root node registration
            {
                RegisterNode(new LayoutNode(node, null));
            }
            else if (node != null)
            {
                var parent = layoutInfo[relations[0].From.Guid];
                RegisterNode(new LayoutNode(node, parent));
            } // else only relationship being added
        }

        private LayoutNode RegisterNode(LayoutNode node) => layoutInfo[node.Node.Guid] = node;

        public LayoutNode GetNode(Guid guid) => layoutInfo[guid];
    }
}