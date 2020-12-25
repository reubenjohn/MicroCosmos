using System;
using Organelles.BirthCanal;
using Organelles.Flagella;
using UnityEngine;
using Util;

namespace Brains
{
    public class KeyboardBrain : AbstractBrain
    {
        public float linearFlagellaSensitivity = 10f;
        public float angularFlagellaSensitivity = 10f;
        private float[] birthCanalLogits;

        private float[] flagellaLogits;

        protected override void Start()
        {
            base.Start();
            flagellaLogits = FindLogits(FlagellaActuator.ActuatorType);
            birthCanalLogits = FindLogits(BirthCanal.ActuatorType);
        }

        private float[] FindLogits(string actuatorType)
        {
            var index = Array.FindIndex(actuators, actuator => actuator.GetActuatorType() == actuatorType);
            return index > -1 ? actuatorLogits[index] : null;
        }

        protected override void React()
        {
            if (flagellaLogits != null) UpdateFlagellaLogits(flagellaLogits);
            if (birthCanalLogits != null) UpdateBirthCanalLogits(birthCanalLogits);
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