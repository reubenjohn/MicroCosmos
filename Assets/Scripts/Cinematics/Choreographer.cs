using System.Collections.Generic;
using System.Diagnostics;
using Genealogy;
using UnityEngine;

namespace Cinematics
{
    public class Choreographer : MonoBehaviour
    {
        public Camera overviewCamera { get; private set; }
        public FocusCamera focusCamera { get; private set; }

        private readonly List<IChoreographerListener> listeners = new List<IChoreographerListener>();

        private void Start()
        {
            overviewCamera = transform.Find("Overview Camera").GetComponent<Camera>();
            focusCamera = transform.Find("Focus Camera").GetComponent<FocusCamera>();
        }

        public void SetFocus(GameObject cell)
        {
            if (cell == null)
            {
                SwitchCamera(overviewCamera);
            }
            else
            {
                SwitchCamera(focusCamera.cam);
                focusCamera.Focus = cell;
            }
        }

        private void SwitchCamera(Camera cam)
        {
            overviewCamera.enabled = false;
            focusCamera.Focus = null;
            if (cam == overviewCamera) overviewCamera.enabled = true;
            else if (cam == focusCamera.cam) focusCamera.enabled = true;

            foreach (var listener in listeners) listener.OnSwitchCamera(cam);
        }

        public void AddListener(IChoreographerListener listener) => listeners.Add(listener);
    }
}