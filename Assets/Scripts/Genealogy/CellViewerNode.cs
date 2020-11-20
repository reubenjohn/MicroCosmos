using UnityEngine;
using UnityEngine.UI;

namespace Genealogy
{
    public class CellViewerNode : MonoBehaviour
    {
        private Text text;
        private CellNode cellNode;

        private void Start()
        {
            text = transform.GetComponent<Text>();
        }

        public CellNode CellNode
        {
            get => cellNode;
            set
            {
                text.name = cellNode.ToString();
            }
        }
    }
}