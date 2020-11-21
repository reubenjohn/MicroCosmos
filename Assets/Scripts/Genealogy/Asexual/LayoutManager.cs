using System;
using System.Collections.Generic;

namespace Genealogy.Asexual
{
    public class LayoutManager : IGenealogyGraphListener
    {
        private readonly Dictionary<Guid, LayoutNode> layoutInfo;

        private readonly List<ILayoutChangeListener<LayoutNode>> listeners =
            new List<ILayoutChangeListener<LayoutNode>>();

        public LayoutManager() : this(new Dictionary<Guid, LayoutNode>())
        {
        }

        public LayoutManager(Dictionary<Guid, LayoutNode> layoutInfo)
        {
            this.layoutInfo = layoutInfo;
        }

        public void OnTransactionComplete(GenealogyGraph genealogyGraph, Node node, List<Relation> relations)
        {
            if (relations.Count == 0) // Root node registration
            {
                RegisterNode(new LayoutNode(listeners, node, null));
            }
            else
            {
                if (node != null)
                {
                    var parent = layoutInfo[relations[0].From.Guid]; // Assume single asexual parent
                    RegisterNode(new LayoutNode(listeners, node, parent));

                    foreach (var listener in listeners) listener.OnAddConnections(relations);
                }
            }
        }

        private LayoutNode RegisterNode(LayoutNode node) => layoutInfo[node.Node.Guid] = node;

        public LayoutNode GetNode(Guid guid) => layoutInfo[guid];

        public void AddListener(ILayoutChangeListener<LayoutNode> layoutListener) => listeners.Add(layoutListener);

        public void RemoveListener(ILayoutChangeListener<LayoutNode> layoutListener) =>
            listeners.Remove(layoutListener);
    }
}