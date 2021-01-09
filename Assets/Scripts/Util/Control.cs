using System;
using UnityEngine;

namespace Util
{
    public static class Control
    {
        public static float BinaryControlStep(float currentValue,
            bool positiveInput, bool negativeInput, float inputSensitivity, float deltaTime)
        {
            var linearTarget = ToLogit(positiveInput) - ToLogit(negativeInput);
            var remainingDistance = linearTarget - currentValue;
            var maxLinearStep = deltaTime * inputSensitivity;
            var linearStep = Mathf.Clamp(remainingDistance, -maxLinearStep, maxLinearStep);
            currentValue += linearStep;
            return currentValue;
        }

        private static float ToLogit(bool boolean) => boolean ? 1 : 0;

        public static float ExponentialDecayControlStep(float currentValue,
            bool positiveInput, bool negativeInput, float inputSensitivity, float deltaTime)
        {
            var linearTarget = ToLogit(positiveInput) - ToLogit(negativeInput);
            currentValue += Mathf.Clamp((linearTarget - currentValue) * inputSensitivity * deltaTime, -1f, 1f);
            return currentValue;
        }

        [Serializable]
        public class BinaryControlVariable
        {
            public float inputSensitivity;

            public BinaryControlVariable(float inputSensitivity)
            {
                this.inputSensitivity = inputSensitivity;
            }

            public float Value { get; set; }

            public float FeedInput(bool positive, bool negative, float deltaTime) =>
                Value = BinaryControlStep(Value, positive, negative, inputSensitivity, deltaTime);
        }

        [Serializable]
        public class ExponentialDecayControlVariable
        {
            public float inputSensitivity;

            public ExponentialDecayControlVariable(float inputSensitivity)
            {
                this.inputSensitivity = inputSensitivity;
            }

            public float Value { get; set; }

            public float FeedInput(bool positive, bool negative, float deltaTime) =>
                Value = ExponentialDecayControlStep(Value, positive, negative, inputSensitivity, deltaTime);
        }
    }
}