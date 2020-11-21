using UnityEngine;

namespace Cinematics
{
    [RequireComponent(typeof(Camera))]
    public class FocusCamera : MonoBehaviour
    {
        public Camera cam { get; private set; }
        private GameObject focus;

        public Vector3 offset = new Vector3(0, 0, -4f);

        public GameObject Focus
        {
            get => focus;
            set
            {
                cam.enabled = value;
                focus = value;
            }
        }

        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        private void Update()
        {
            if (cam.enabled)
            {
                transform.position = focus.transform.position + offset;
            }
        }
    }
}