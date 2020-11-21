using UnityEngine;

namespace Brains
{
    public class DivinePossession : MonoBehaviour
    {
        private KeyboardBrain currentTarget;

        public void SetPossessionTarget(Cell.Cell targetCell)
        {
            if (currentTarget != null)
            {
                var cellObj = currentTarget.gameObject;
                Destroy(currentTarget);
                currentTarget = null;
                cellObj.GetComponentInChildren<AbstractBrain>().enabled = true;
            }

            if (targetCell != null)
            {
                targetCell.GetComponentInChildren<AbstractBrain>().enabled = false;
                currentTarget = targetCell.gameObject.AddComponent<KeyboardBrain>();
            }
        }
    }
}