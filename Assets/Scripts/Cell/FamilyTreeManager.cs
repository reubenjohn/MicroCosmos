using Genealogy;
using Genealogy.AsexualFamilyTree;
using UnityEngine;

namespace Cell
{
    public class FamilyTreeManager : MonoBehaviour
    {
        private readonly FamilyTree familyTree = new FamilyTree();
        [SerializeField] private FamilyTreeViewer viewer;

        private void Start()
        {
            if (viewer)
            {
                var layoutManager = new LayoutManager();
                familyTree.AddListener(layoutManager);
                layoutManager.AddListener(viewer);
            }

            familyTree.RegisterRootNode(new CellNode(null, "Cell"));
        }

        public void RegisterAsexualCellBirth(Node[] parentGenealogyNodes, Cell child)
        {
            parentGenealogyNodes = parentGenealogyNodes.Length > 0 ? parentGenealogyNodes : new[] {familyTree.rootNode};
            child.name = NamingSystem.GetChildName(familyTree, (CellNode) parentGenealogyNodes[0]);
            child.genealogyNode = new CellNode(child.gameObject, child.name);
            familyTree.RegisterReproduction(parentGenealogyNodes, child.genealogyNode);
        }
    }
}