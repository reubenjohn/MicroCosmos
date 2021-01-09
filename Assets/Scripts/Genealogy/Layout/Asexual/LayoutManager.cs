using System;
using System.Collections.Generic;
using System.Linq;
using Genealogy.Graph;

namespace Genealogy.Layout.Asexual
{
    public class LayoutManager : IGenealogyGraphListener
    {
        private readonly Dictionary<Guid, LayoutNode> layoutInfo;

        private readonly List<ILayoutChangeListener<LayoutNode>> listeners =
            new List<ILayoutChangeListener<LayoutNode>>();

        private LayoutNode rootNode;

        public LayoutManager() : this(new Dictionary<Guid, LayoutNode>()) { }

        public LayoutManager(Dictionary<Guid, LayoutNode> layoutInfo)
        {
            this.layoutInfo = layoutInfo;
        }

        public bool LiveLayoutEnabled { get; set; }

        public void OnTransactionComplete(GenealogyGraph genealogyGraph, Node node, List<Relation> relations)
        {
            if (relations.Count == 0) // Root node registration
            {
                rootNode = RegisterNode(new LayoutNode(listeners, node, null));
            }
            else
            {
                if (relations.Count > 1) throw new InvalidOperationException("Currently unsupported");
                var parent = layoutInfo[relations[0].From.Guid]; // Assume single asexual parent
                var newNode = RegisterNode(new LayoutNode(listeners, node, parent));
                foreach (var listener in listeners) listener.OnAddConnections(relations);
                if (node.NodeType == NodeType.Death) PruneAncestry(newNode.Parent);
            }

            if (LiveLayoutEnabled)
                rootNode.RecalculateLayout();
        }

        public void OnClear()
        {
            foreach (var listener in listeners) listener.OnClear();
            layoutInfo.Clear();
        }

        private void PruneAncestry(LayoutNode cellNode)
        {
            var myDeathNode = cellNode.children.Find(child => child.Node.NodeType == NodeType.Death);
            if (myDeathNode != null)
            {
                var iAnyLivingChildren = cellNode.children.Any(child => child.Node.NodeType == NodeType.Reproduction);
                if (!iAnyLivingChildren)
                {
                    var reproductionNode = cellNode.Parent;
                    var parentNode = reproductionNode.Parent;

                    myDeathNode.Remove();
                    cellNode.Remove();
                    reproductionNode.Remove();
                    PruneAncestry(parentNode);
                }
            }
        }

        private LayoutNode RegisterNode(LayoutNode node)
        {
            return layoutInfo[node.Node.Guid] = node;
        }

        public LayoutNode GetNode(Guid guid) => layoutInfo[guid];

        public void AddListener(ILayoutChangeListener<LayoutNode> layoutListener)
        {
            listeners.Add(layoutListener);
        }

        public void RemoveListener(ILayoutChangeListener<LayoutNode> layoutListener)
        {
            listeners.Remove(layoutListener);
        }

        public void RecalculateLayout()
        {
            rootNode.RecalculateLayout();
        }
    }
}