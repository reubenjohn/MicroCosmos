using UnityEngine.EventSystems;

namespace Genealogy
{
    public interface IGraphViewerListener
    {
        void OnSelectNode(ViewerNode viewerNode, PointerEventData eventData);
        void OnDeselectNode(ViewerNode viewerNode, PointerEventData eventData);
    }
}