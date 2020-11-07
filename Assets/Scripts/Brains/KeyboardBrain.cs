using Actuators;
using UnityEngine;

namespace Brains
{
    public class KeyboardBrain : Brain, IBrain
    {
        public float linearFlagellaSensitivity = 0.1f;
        public float angularFlagellaSensitivity = 0.1f;

        public new void Start()
        {
            base.Start();
        }


        public override void Update()
        {
            UpdateFlagellaLogits(actuatorLogits[0]);

            var birthCanal = transform.GetComponentInChildren<BirthCanal>();
            if (Input.GetKeyDown(KeyCode.B))
                birthCanal.GiveBirth();

            base.Update();
        }

        private void UpdateFlagellaLogits(float[] logits)
        {
            logits[0] = BinaryControlStep(logits[0], Input.GetKey(KeyCode.W), Input.GetKey(KeyCode.S),
                linearFlagellaSensitivity);
            logits[1] = BinaryControlStep(logits[1], Input.GetKey(KeyCode.A), Input.GetKey(KeyCode.D),
                angularFlagellaSensitivity);
        }

        private static float BinaryControlStep(float value, bool positiveInput, bool negativeInput,
            float inputSensitivity)
        {
            var linearTarget = ToLogit(positiveInput) - ToLogit(negativeInput);
            var remainingDistance = linearTarget - value;
            var maxLinearStep = Time.deltaTime * inputSensitivity;
            var linearStep = Mathf.Clamp(remainingDistance, -maxLinearStep, maxLinearStep);
            value += linearStep;
            return value;
        }

        private static float ToLogit(bool boolean)
        {
            return boolean ? 1 : 0;
        }
    }
}