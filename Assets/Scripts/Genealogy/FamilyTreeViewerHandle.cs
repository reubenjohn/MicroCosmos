using Genealogy.AsexualFamilyTree;
using UnityEngine;

namespace Genealogy
{
    public class FamilyTreeViewerHandle
    {
        public readonly LayoutNode layout;
        public readonly GameObject viewerObj;

        public FamilyTreeViewerHandle(LayoutNode layout, GameObject viewerObj)
        {
            this.layout = layout;
            this.viewerObj = viewerObj;
        }
    }
}