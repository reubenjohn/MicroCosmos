using Genealogy.Asexual;
using UnityEngine;

namespace Genealogy
{
    public abstract class ViewerNode : MonoBehaviour
    {
        private RectTransform rectTransform;
        public abstract Node GenealogyNode { get; set; }

        public virtual void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        protected static T InstantiateNode<T>(string resourcePath, Transform parentTransform, Node genealogyNode)
            where T : ViewerNode
        {
            var prefab = Resources.Load<GameObject>(resourcePath);
            var obj = Instantiate(prefab, parentTransform);
            var viewerNode = obj.GetComponent<T>();
            viewerNode.Start();
            viewerNode.GenealogyNode = genealogyNode;
            return viewerNode;
        }

        public virtual void OnUpdate(LayoutNode layout)
        {
            rectTransform.localPosition = layout.Center * GenealogyGraphViewer.DisplayScale;
            gameObject.name = layout.Node.ToString();
        }
    }
}