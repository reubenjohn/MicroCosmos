using Cell;
using Genealogy.Asexual;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Genealogy
{
    [RequireComponent(typeof(Image))]
    public class CellViewerNode : ViewerNode, IPointerClickHandler
    {
        private Image image;
        private Text text;
        private CellNode cellNode;

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

        public static CellViewerNode InstantiateNode(Transform parentTransform, CellNode cellNode) =>
            InstantiateNode<CellViewerNode>("UI/CellViewerNode", parentTransform, cellNode);

        public override void OnUpdate(LayoutNode layout)
        {
            base.OnUpdate(layout);
            text.text = layout.Node.ToString();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 1)
            {
                GetComponentInParent<GenealogyGraphViewer>().OnClick(this, eventData);
            }
            // else if (eventData.clickCount == 2)
            // {
            // }
        }

        public void SetSelectedState(bool selectedState)
        {
            image.color = selectedState ? Color.green : Color.white;
        }
    }
}