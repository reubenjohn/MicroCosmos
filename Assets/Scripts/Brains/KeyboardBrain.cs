﻿using UnityEngine;
using Util;

namespace Brains
{
    public class KeyboardBrain : AbstractBrain
    {
        public float linearFlagellaSensitivity = 10f;
        public float angularFlagellaSensitivity = 10f;

        protected override void React()
        {
            UpdateFlagellaLogits(actuatorLogits[1]);
            UpdateBirthCanalLogits(actuatorLogits[2]);
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