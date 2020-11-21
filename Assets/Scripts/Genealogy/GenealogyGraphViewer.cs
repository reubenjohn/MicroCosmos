﻿using System;
using System.Collections;
using System.Collections.Generic;
using Genealogy.Asexual;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Genealogy
{
    [RequireComponent(typeof(Canvas))]
    public class GenealogyGraphViewer : MonoBehaviour, ILayoutChangeListener<LayoutNode>, IChoreographerListener
    {
        public static readonly Vector2 DisplayScale = new Vector2(60, 20);

        private Canvas canvas;
        private Transform genealogyGraphContentTransform;

        private ViewerNode currentSelectedNode = null;

        private readonly Dictionary<Guid, GenealogyGraphViewerHandle> viewerNodes =
            new Dictionary<Guid, GenealogyGraphViewerHandle>();

        private readonly List<IGraphViewerListener> listeners = new List<IGraphViewerListener>();

        private void Start()
        {
            canvas = GetComponent<Canvas>();
            genealogyGraphContentTransform = GameObject.Find("_Family Tree Content").transform;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                StartCoroutine(SetVisibility(!canvas.enabled));
        }

        private IEnumerator SetVisibility(bool visibility)
        {
            canvas.enabled = visibility;
            foreach (Transform connection in ConnectionManager.instance.transform)
            {
                SetVisibility(connection.GetComponent<Connection>(), visibility);
                yield return null;
            }
        }

        private void SetVisibility(Connection connection, bool visibility)
        {
            connection.enabled = visibility;
            connection.line.enabled = visibility;
        }

        public void OnAddNode(LayoutNode layout)
        {
            var viewerNode = InstantiateNode(layout.Node);
            RegisterViewerNode(new GenealogyGraphViewerHandle(layout, viewerNode));
        }

        private ViewerNode InstantiateNode(Node node)
        {
            switch (node.NodeType)
            {
                case NodeType.Cell:
                    return CellViewerNode.InstantiateNode(genealogyGraphContentTransform, (CellNode) node);
                default:
                    return ReproductionViewerNode.InstantiateNode(genealogyGraphContentTransform, (Reproduction) node);
            }
        }

        public void OnAddConnections(List<Relation> relations)
        {
            foreach (var relation in relations)
            {
                var from = viewerNodes[relation.From.Guid].viewerNode.GetComponent<RectTransform>();
                var to = viewerNodes[relation.To.Guid].viewerNode.GetComponent<RectTransform>();
                var connection = ConnectionManager.CreateConnection(from, to);
                SetVisibility(connection, canvas.enabled);
            }
        }

        public void OnUpdateNode(LayoutNode layout) => viewerNodes[layout.Node.Guid].OnUpdate();


        private void RegisterViewerNode(GenealogyGraphViewerHandle viewerHandle)
        {
            viewerNodes[viewerHandle.layout.Node.Guid] = viewerHandle;
            viewerHandle.OnUpdate();
        }

        public void OnClick(ViewerNode viewerNode, PointerEventData eventData)
        {
            // Select
            if (currentSelectedNode == null)
            {
                currentSelectedNode = viewerNode;
                SelectNode(viewerNode, eventData);
            }
            // Deselect
            else if (currentSelectedNode == viewerNode)
            {
                currentSelectedNode = null;
                DeselectNode(viewerNode, eventData);
            }
            // Switch
            else
            {
                var previousSelectedNode = currentSelectedNode;
                currentSelectedNode = viewerNode;
                DeselectNode(previousSelectedNode, eventData);
                SelectNode(viewerNode, eventData);
            }
        }

        private void DeselectNode(ViewerNode viewerNode, PointerEventData eventData)
        {
            if (viewerNode.GenealogyNode.NodeType == NodeType.Cell)
                ((CellViewerNode) viewerNode).SetSelectedState(false);
            foreach (var listener in listeners)
                listener.OnDeselectNode(viewerNode, eventData);
        }

        private void SelectNode(ViewerNode viewerNode, PointerEventData eventData)
        {
            if (viewerNode.GenealogyNode.NodeType == NodeType.Cell)
                ((CellViewerNode) viewerNode).SetSelectedState(true);
            foreach (var listener in listeners)
                listener.OnSelectNode(viewerNode, eventData);
        }

        public void AddListener(IGraphViewerListener graphViewerListener) => listeners.Add(graphViewerListener);

        public void OnSwitchCamera(Camera cam) => canvas.worldCamera = cam;
    }
}