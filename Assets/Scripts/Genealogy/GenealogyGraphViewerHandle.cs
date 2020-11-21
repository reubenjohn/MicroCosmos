using Genealogy.Asexual;

namespace Genealogy
{
    public class GenealogyGraphViewerHandle
    {
        public readonly LayoutNode layout;
        public readonly ViewerNode viewerNode;

        public GenealogyGraphViewerHandle(LayoutNode layout, ViewerNode viewerNode)
        {
            this.layout = layout;
            this.viewerNode = viewerNode;
        }

        public void OnUpdate() => viewerNode.OnUpdate(layout);
    }
}