using Brains;
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
        private readonly GenealogyGraph genealogyGraph = new GenealogyGraph();
        private CellColony CellColony { get; set; }
        private DivinePossession divinePossession;
        [SerializeField] private Choreographer Choreographer;
        [SerializeField] private GenealogyGraphViewer viewer;

        private void Start()
        {
            CellColony = GetComponent<CellColony>();
            divinePossession = GetComponent<DivinePossession>();
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
                var targetCell = CellColony.FindCell(viewerNode.GenealogyNode.Guid);
                if (targetCell != null)
                {
                    Choreographer.SetFocus(targetCell.gameObject);
                    divinePossession.SetPossessionTarget(targetCell);
                    targetCell.IsInFocus = true;
                }
            }
        }

        public void OnDeselectNode(ViewerNode viewerNode, PointerEventData eventData)
        {
            if (viewerNode.GenealogyNode.NodeType == NodeType.Cell)
            {
                Choreographer.SetFocus(null);
                divinePossession.SetPossessionTarget(null);
                var targetCell = CellColony.FindCell(viewerNode.GenealogyNode.Guid);
                if (targetCell != null)
                    targetCell.IsInFocus = false;
            }
        }
    }
}