using Genealogy;
using Genealogy.Asexual;
using UnityEngine;

namespace Cell
{
    public class GenealogyGraphManager : MonoBehaviour
    {
        private readonly GenealogyGraph genealogyGraph = new GenealogyGraph();
        [SerializeField] private GenealogyGraphViewer viewer;

        private void Start()
        {
            if (viewer)
            {
                var layoutManager = new LayoutManager();
                layoutManager.AddListener(viewer);
                genealogyGraph.AddListener(layoutManager);
            }
            else
            {
                Debug.LogWarning("No genealogy graph viewer is listening in on the genealogy graph");
            }

            genealogyGraph.RegisterRootNode(new CellNode(null, "Cell"));
        }

        public void RegisterAsexualCellBirth(Node[] parentGenealogyNodes, Cell child)
        {
            parentGenealogyNodes = parentGenealogyNodes.Length > 0 ? parentGenealogyNodes : new[] {genealogyGraph.rootNode};
            child.name = NamingSystem.GetChildName(genealogyGraph, (CellNode) parentGenealogyNodes[0]);
            child.genealogyNode = new CellNode(child.gameObject, child.name);
            genealogyGraph.RegisterReproduction(parentGenealogyNodes, child.genealogyNode);
        }
    }
}