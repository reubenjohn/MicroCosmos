using UnityEngine;

namespace Cinematics
{
    [RequireComponent(typeof(Camera))]
    public class FocusCamera : MonoBehaviour
    {
        public Vector3 offset = new Vector3(0, 0, -2f);
        private GameObject focus;
        public Camera cam { get; private set; }

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
                transform.position = focus.transform.position + offset * focus.transform.localScale.magnitude;
        }
    }
}