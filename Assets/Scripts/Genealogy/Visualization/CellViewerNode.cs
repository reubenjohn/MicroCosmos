using Genealogy.Graph;
using Genealogy.Layout.Asexual;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Genealogy.Visualization
{
    [RequireComponent(typeof(Image))]
    public class CellViewerNode : ViewerNode, IPointerClickHandler
    {
        private CellNode cellNode;
        private Image image;
        private Text text;

        public override Node GenealogyNode
        {
            get => cellNode;
            set => cellNode = (CellNode) value;
        }

        public override void Start()
        {
            base.Start();
            text = transform.GetComponentInChildren<Text>();
            image = transform.GetComponent<Image>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 1) GetComponentInParent<GenealogyGraphViewer>().OnClick(this, eventData);
            // else if (eventData.clickCount == 2)
            // {
            // }
        }

        public static CellViewerNode InstantiateNode(Transform parentTransform, CellNode cellNode)
        {
            return InstantiateNode<CellViewerNode>("UI/CellViewerNode", parentTransform, cellNode);
        }

        public override void OnUpdate(LayoutNode layout)
        {
            base.OnUpdate(layout);
            text.text = layout.Node.ToString();
        }

        public void SetSelectedState(bool selectedState)
        {
            image.color = selectedState ? Color.green : Color.white;
        }
    }
}