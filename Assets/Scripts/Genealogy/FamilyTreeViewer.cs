using System;
using System.Collections.Generic;
using Genealogy.AsexualFamilyTree;
using UnityEngine;
using UnityEngine.UI;

namespace Genealogy
{
    public class FamilyTreeViewer : MonoBehaviour, ILayoutChangeListener<LayoutNode>
    {
        private GameObject cellViewerNodeTemplate;
        private GameObject reproductionViewerNodeTemplate;

        private readonly Dictionary<Guid, ViewerHandle> viewerNodes = new Dictionary<Guid, ViewerHandle>();

        private static readonly Vector2 DisplayScale = new Vector2(60, 20);

        private void Start()
        {
            cellViewerNodeTemplate = Resources.Load<GameObject>("UI/CellViewerNode");
            reproductionViewerNodeTemplate = Resources.Load<GameObject>("UI/ReproductionViewerNode");
        }

        public void OnAddNode(LayoutNode layout)
        {
            GameObject viewerNode;
            switch (layout.Node.NodeType)
            {
                case NodeType.Cell:
                    viewerNode = NewCellViewerNode((CellNode) layout.Node);
                    break;
                default:
                    viewerNode = NewReproductionViewerNode();
                    break;
            }

            var viewerHandle = new ViewerHandle(layout, viewerNode);
            RegisterViewerNode(viewerHandle);
            UpdateViewerNode(viewerHandle);
        }

        private GameObject NewCellViewerNode(CellNode cellNode)
        {
            var viewerNode = Instantiate(cellViewerNodeTemplate, transform);
            viewerNode.GetComponentInChildren<Text>().text = cellNode.ToString();
            return viewerNode;
        }

        private GameObject NewReproductionViewerNode()
        {
            var viewerNode = Instantiate(reproductionViewerNodeTemplate, transform);
            return viewerNode;
        }

        public void OnAddConnections(List<Relation> relations)
        {
            foreach (var relation in relations)
            {
                var from = viewerNodes[relation.From.Guid].ViewerObj.GetComponent<RectTransform>();
                var to = viewerNodes[relation.To.Guid].ViewerObj.GetComponent<RectTransform>();
                ConnectionManager.CreateConnection(from, to);
            }
        }

        public void OnUpdateNode(LayoutNode layout) => UpdateViewerNode(viewerNodes[layout.Node.Guid]);


        private void UpdateViewerNode(ViewerHandle viewerHandle)
        {
            var rectTransform = viewerHandle.ViewerObj.GetComponent<RectTransform>();
            rectTransform.localPosition = viewerHandle.Layout.Center * DisplayScale;
            viewerHandle.ViewerObj.name = viewerHandle.Layout.Node.ToString();
        }

        private void RegisterViewerNode(ViewerHandle viewerHandle) =>
            viewerNodes[viewerHandle.Layout.Node.Guid] = viewerHandle;

        public class ViewerHandle
        {
            public LayoutNode Layout { get; }
            public GameObject ViewerObj { get; }

            public ViewerHandle(LayoutNode layout, GameObject viewerObj)
            {
                Layout = layout;
                ViewerObj = viewerObj;
            }
        }
    }
}