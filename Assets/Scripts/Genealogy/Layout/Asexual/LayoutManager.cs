﻿using System;
using System.Collections.Generic;
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

        public void OnAddTransactionComplete(GenealogyGraph genealogyGraph, Node node, Relation[] relations)
        {
            if (relations.Length == 0) // Root node registration
            {
                rootNode = RegisterNode(new LayoutNode(listeners, node, null));
            }
            else
            {
                if (relations.Length > 1) throw new InvalidOperationException("Currently unsupported");
                var parent = layoutInfo[relations[0].From.Guid]; // Assume single asexual parent
                RegisterNode(new LayoutNode(listeners, node, parent));
                foreach (var listener in listeners) listener.OnAddConnections(relations);
            }

            if (LiveLayoutEnabled)
                rootNode.RecalculateLayout();
        }

        public void OnRemoveTransactionComplete(GenealogyGraph genealogyGraph, Node node, Relation[] relations) =>
            layoutInfo[node.Guid].Remove();

        public void OnClear()
        {
            foreach (var listener in listeners) listener.OnClear();
            layoutInfo.Clear();
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