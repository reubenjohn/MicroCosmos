using System;
using System.Collections;
using System.Collections.Generic;
using Genealogy.Asexual;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Genealogy
{
    [RequireComponent(typeof(Canvas))]
    public class GenealogyGraphViewer : MonoBehaviour, ILayoutChangeListener<LayoutNode>
    {
        private Canvas canvas;
        private GameObject cellViewerNodeTemplate;
        private GameObject reproductionViewerNodeTemplate;
        private Transform genealogyGraphContentTransform;

        private readonly Dictionary<Guid, GenealogyGraphViewerHandle> viewerNodes =
            new Dictionary<Guid, GenealogyGraphViewerHandle>();

        private static readonly Vector2 DisplayScale = new Vector2(60, 20);

        private void Start()
        {
            canvas = GetComponent<Canvas>();
            genealogyGraphContentTransform = GameObject.Find("_Family Tree Content").transform;
            cellViewerNodeTemplate = Resources.Load<GameObject>("UI/CellViewerNode");
            reproductionViewerNodeTemplate = Resources.Load<GameObject>("UI/ReproductionViewerNode");
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

            var viewerHandle = new GenealogyGraphViewerHandle(layout, viewerNode);
            RegisterViewerNode(viewerHandle);
            UpdateViewerNode(viewerHandle);
        }

        private GameObject NewCellViewerNode(CellNode cellNode)
        {
            var viewerNode = Instantiate(cellViewerNodeTemplate, genealogyGraphContentTransform);
            viewerNode.GetComponentInChildren<Text>().text = cellNode.ToString();
            return viewerNode;
        }

        private GameObject NewReproductionViewerNode() =>
            Instantiate(reproductionViewerNodeTemplate, genealogyGraphContentTransform);

        public void OnAddConnections(List<Relation> relations)
        {
            foreach (var relation in relations)
            {
                var from = viewerNodes[relation.From.Guid].viewerObj.GetComponent<RectTransform>();
                var to = viewerNodes[relation.To.Guid].viewerObj.GetComponent<RectTransform>();
                var connection = ConnectionManager.CreateConnection(from, to);
                SetVisibility(connection, canvas.enabled);
            }
        }

        public void OnUpdateNode(LayoutNode layout) => UpdateViewerNode(viewerNodes[layout.Node.Guid]);


        private void UpdateViewerNode(GenealogyGraphViewerHandle viewerHandle)
        {
            var rectTransform = viewerHandle.viewerObj.GetComponent<RectTransform>();
            rectTransform.localPosition = viewerHandle.layout.Center * DisplayScale;
            viewerHandle.viewerObj.name = viewerHandle.layout.Node.ToString();
        }

        private void RegisterViewerNode(GenealogyGraphViewerHandle viewerHandle) =>
            viewerNodes[viewerHandle.layout.Node.Guid] = viewerHandle;
    }
}