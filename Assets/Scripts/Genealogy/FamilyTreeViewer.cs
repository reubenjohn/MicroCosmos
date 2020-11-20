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

        private readonly Dictionary<Guid, ViewerHandle> viewerNodes = new Dictionary<Guid, ViewerHandle>();

        private static readonly Vector2 DisplayScale = new Vector2(60, 40);

        private void Start()
        {
            cellViewerNodeTemplate = Resources.Load<GameObject>("UI/CellViewerNode");
        }

        public void OnAddNode(LayoutNode layout)
        {
            var viewerNode = Instantiate(cellViewerNodeTemplate, transform);
            var nodeName = viewerNode.name = layout.Node.ToString();
            viewerNode.name = nodeName;
            viewerNode.GetComponentInChildren<Text>().text = nodeName;

            var viewerHandle = new ViewerHandle(layout, viewerNode);
            RegisterViewerNode(viewerHandle);
            UpdateViewerNode(viewerHandle);
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