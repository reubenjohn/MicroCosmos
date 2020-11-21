using Genealogy.Asexual;
using UnityEngine;

namespace Genealogy
{
    public class GenealogyGraphViewerHandle
    {
        public readonly LayoutNode layout;
        public readonly GameObject viewerObj;

        public GenealogyGraphViewerHandle(LayoutNode layout, GameObject viewerObj)
        {
            this.layout = layout;
            this.viewerObj = viewerObj;
        }
    }
}