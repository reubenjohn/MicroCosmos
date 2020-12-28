using System;
using System.Collections;
using System.Collections.Generic;
using Cinematics;
using Genealogy.Graph;
using Genealogy.Layout;
using Genealogy.Layout.Asexual;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Genealogy.Visualization
{
    [RequireComponent(typeof(Canvas))]
    public class GenealogyGraphViewer : MonoBehaviour, ILayoutChangeListener<LayoutNode>, IChoreographerListener
    {
        public static readonly Vector2 DisplayScale = new Vector2(60, 20);

        private readonly List<IGraphViewerListener> listeners = new List<IGraphViewerListener>();

        private readonly Dictionary<Guid, GenealogyGraphViewerHandle> viewerHandles =
            new Dictionary<Guid, GenealogyGraphViewerHandle>();

        private Canvas canvas;

        private ViewerNode currentSelectedNode;
        private Transform genealogyGraphContentTransform;
        private ScrollRect scrollRect;

        private void Start()
        {
            canvas = GetComponent<Canvas>();
            genealogyGraphContentTransform = GameObject.Find("_Family Tree Content").transform;
            scrollRect = GameObject.Find("_Family Tree").GetComponent<ScrollRect>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                StartCoroutine(SetVisibility(!canvas.enabled));
        }

        public void OnSwitchCamera(Camera cam)
        {
            canvas.worldCamera = cam;
        }

        public void OnAddNode(LayoutNode layout)
        {
            var viewerNode = InstantiateNode(layout.Node);
            RegisterViewerNode(new GenealogyGraphViewerHandle(layout, viewerNode));
        }

        public void OnAddConnections(List<Relation> relations)
        {
            foreach (var relation in relations)
            {
                var viewerNode = viewerHandles[relation.From.Guid].viewerNode;
                var from = viewerNode.GetComponent<RectTransform>();
                var to = viewerHandles[relation.To.Guid].viewerNode.GetComponent<RectTransform>();
                var connection = ConnectionManager.CreateConnection(from, to);
                SetVisibility(connection, canvas.enabled);
                if (relation.RelationType == RelationType.Death && viewerNode == currentSelectedNode)
                    DeselectNode(viewerNode, null);
            }
        }

        public void OnClear()
        {
            if (currentSelectedNode)
                foreach (var listener in listeners)
                    listener.OnDeselectNode(currentSelectedNode, null);
            currentSelectedNode = default;
            foreach (var viewerHandle in viewerHandles.Values) viewerHandle.OnDestroy();
            ConnectionManager.CleanConnections();
            viewerHandles.Clear();
        }

        public void OnUpdateNode(LayoutNode layout)
        {
            viewerHandles[layout.Node.Guid].OnUpdate();
        }

        private IEnumerator SetVisibility(bool visibility)
        {
            canvas.enabled = scrollRect.enabled = visibility;
            var i = 0;
            foreach (Transform connection in ConnectionManager.instance.transform)
            {
                SetVisibility(connection.GetComponent<Connection>(), visibility);
                if (i++ % 500 == 0)
                    yield return null;
            }
        }

        private void SetVisibility(Connection connection, bool visibility)
        {
            connection.enabled = visibility;
            connection.line.enabled = visibility;
        }

        private ViewerNode InstantiateNode(Node node)
        {
            switch (node.NodeType)
            {
                case NodeType.Cell:
                    return CellViewerNode.InstantiateNode(genealogyGraphContentTransform, (CellNode) node);
                case NodeType.Death:
                    return CellDeathViewerNode.InstantiateNode(genealogyGraphContentTransform, (CellDeath) node);
                default:
                    return ReproductionViewerNode.InstantiateNode(genealogyGraphContentTransform, (Reproduction) node);
            }
        }


        private void RegisterViewerNode(GenealogyGraphViewerHandle viewerHandle)
        {
            viewerHandles[viewerHandle.layout.Node.Guid] = viewerHandle;
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

        public void AddListener(IGraphViewerListener graphViewerListener)
        {
            listeners.Add(graphViewerListener);
        }
    }
}