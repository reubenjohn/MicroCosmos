using Cinematics;
using Genealogy;
using Genealogy.Asexual;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cell
{
    [RequireComponent(typeof(CellColony))]
    public class GenealogyGraphManager : MonoBehaviour, IGraphViewerListener
    {
        public CellColony CellColony { get; private set; }
        public readonly GenealogyGraph genealogyGraph = new GenealogyGraph();
        [SerializeField] private Choreographer Choreographer;
        [SerializeField] private GenealogyGraphViewer viewer;

        private void Start()
        {
            CellColony = GetComponent<CellColony>();
            if (viewer)
            {
                Choreographer.AddListener(viewer);

                var layoutManager = new LayoutManager();
                layoutManager.AddListener(viewer);
                genealogyGraph.AddListener(layoutManager);
                viewer.AddListener(this);
            }
            else
            {
                Debug.LogWarning("No genealogy graph viewer is listening in on the genealogy graph");
            }

            genealogyGraph.RegisterRootNode(new CellNode("Cell"));
        }

        public void RegisterAsexualCellBirth(Node[] parentGenealogyNodes, Cell child)
        {
            parentGenealogyNodes =
                parentGenealogyNodes.Length > 0 ? parentGenealogyNodes : new[] {genealogyGraph.rootNode};
            child.name = NamingSystem.GetChildName(genealogyGraph, (CellNode) parentGenealogyNodes[0]);
            child.genealogyNode = new CellNode(child.name);
            genealogyGraph.RegisterReproduction(parentGenealogyNodes, child.genealogyNode);
        }

        public void OnSelectNode(ViewerNode viewerNode, PointerEventData eventData)
        {
            if (viewerNode.GenealogyNode.NodeType == NodeType.Cell)
            {
                Choreographer.SetFocus(CellColony.FindCell(viewerNode.GenealogyNode.Guid)?.gameObject);
            }
        }

        public void OnDeselectNode(ViewerNode viewerNode, PointerEventData eventData)
        {
            if (viewerNode.GenealogyNode.NodeType == NodeType.Cell)
            {
                Choreographer.SetFocus(null);
            }
        }
    }
}