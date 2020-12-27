using System.Collections.Generic;
using Cell;
using UnityEngine;

namespace Cinematics
{
    public class Choreographer : MonoBehaviour, ICellSelectionListener
    {
        private readonly List<IChoreographerListener> listeners = new List<IChoreographerListener>();
        public Camera overviewCamera { get; private set; }
        public FocusCamera focusCamera { get; private set; }

        private void Start()
        {
            overviewCamera = transform.Find("Overview Camera").GetComponent<Camera>();
            focusCamera = transform.Find("Focus Camera").GetComponent<FocusCamera>();
        }

        public void OnCellSelectionChange(Cell.Cell cell, bool select)
        {
            if (select)
            {
                SwitchCamera(focusCamera.cam);
                focusCamera.Focus = cell.gameObject;
            }
            else
            {
                SwitchCamera(overviewCamera);
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