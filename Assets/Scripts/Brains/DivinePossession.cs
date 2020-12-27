using Cell;
using UnityEngine;

namespace Brains
{
    public class DivinePossession : MonoBehaviour, ICellSelectionListener
    {
        private Cell.Cell currentPossession;
        private Cell.Cell currentSelection;
        private bool possessionEnabled;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                possessionEnabled = !possessionEnabled;
                if (currentSelection != null)
                    SetPossession(currentSelection.gameObject, possessionEnabled);
            }
        }

        public void OnCellSelectionChange(Cell.Cell cell, bool select)
        {
            SetPossession(cell.gameObject, select && possessionEnabled);
            currentSelection = select ? cell : null;
        }

        private void SetPossession(GameObject target, bool possess)
        {
            var keyboardBrain = target.GetComponentInChildren<KeyboardBrain>();
            if (keyboardBrain == null && possess)
                target.AddComponent<KeyboardBrain>();
            foreach (var abstractBrain in target.GetComponentsInChildren<AbstractBrain>())
                abstractBrain.enabled = abstractBrain is KeyboardBrain ? possess : !possess;
            if (keyboardBrain != null && !possess)
                Destroy(keyboardBrain);
        }
    }
}