using System;
using Organelles.BirthCanal;
using Organelles.Flagella;
using Organelles.Orifice;
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
        private float[] orificeLogits;

        private UpdateLogits updateLogits;

        protected override void Start()
        {
            base.Start();
            flagellaLogits = FindLogits(FlagellaActuator.ActuatorType);
            birthCanalLogits = FindLogits(BirthCanal.ActuatorType);
            orificeLogits = FindLogits(Orifice.ActuatorType);

            if (flagellaLogits != null) updateLogits += UpdateFlagellaLogits;
            if (birthCanalLogits != null) updateLogits += UpdateBirthCanalLogits;
            if (orificeLogits != null) updateLogits += UpdateOrificeLogits;
        }

        private float[] FindLogits(string actuatorType)
        {
            var index = Array.FindIndex(actuators, actuator => actuator.GetActuatorType() == actuatorType);
            return index > -1 ? actuatorLogits[index] : null;
        }

        protected override void React() => updateLogits();

        private void UpdateFlagellaLogits()
        {
            flagellaLogits[0] = Control.BinaryControlStep(flagellaLogits[0], Input.GetKey(KeyCode.W),
                Input.GetKey(KeyCode.S),
                linearFlagellaSensitivity, Time.deltaTime);
            flagellaLogits[1] = Control.BinaryControlStep(flagellaLogits[1], Input.GetKey(KeyCode.A),
                Input.GetKey(KeyCode.D),
                angularFlagellaSensitivity, Time.deltaTime);
        }

        private void UpdateBirthCanalLogits()
        {
            birthCanalLogits[0] = Input.GetKey(KeyCode.B) ? 1 : -1;
        }

        private void UpdateOrificeLogits()
        {
            orificeLogits[0] = Input.GetKey(KeyCode.Space) ? 1 : -1;
        }

        private delegate void UpdateLogits();
    }
}