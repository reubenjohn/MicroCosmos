using UnityEngine;

namespace Brains
{
    public class KeyboardBrain : AbstractBrain, IBrain
    {
        public float linearFlagellaSensitivity = 0.1f;
        public float angularFlagellaSensitivity = 0.1f;

        public override void React(float[][] sensoryLogits)
        {
            UpdateFlagellaLogits(actuatorLogits[0]);
            UpdateBirthCanalLogits(actuatorLogits[1]);
        }

        private void UpdateFlagellaLogits(float[] logits)
        {
            logits[0] = Control.BinaryControlStep(logits[0], Input.GetKey(KeyCode.W), Input.GetKey(KeyCode.S),
                linearFlagellaSensitivity, Time.deltaTime);
            logits[1] = Control.BinaryControlStep(logits[1], Input.GetKey(KeyCode.A), Input.GetKey(KeyCode.D),
                angularFlagellaSensitivity, Time.deltaTime);
        }

        private void UpdateBirthCanalLogits(float[] logits)
        {
            logits[0] = Input.GetKey(KeyCode.B) ? 1 : -1;
        }
    }
}