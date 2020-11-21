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
                SetPossession(currentTarget.gameObject, false);
                Destroy(currentTarget);
                currentTarget = null;
            }

            if (targetCell != null)
            {
                SetPossession(targetCell.gameObject, true);
                currentTarget = targetCell.gameObject.AddComponent<KeyboardBrain>();
            }
        }

        private void SetPossession(GameObject target, bool possess)
        {
            foreach (var abstractBrain in target.GetComponentsInChildren<AbstractBrain>())
                abstractBrain.enabled = abstractBrain is KeyboardBrain ? possess : !possess;
        }
    }
}