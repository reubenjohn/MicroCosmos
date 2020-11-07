using System;
using UnityEngine;

public class Control
{
    [Serializable]
    public class BinaryControlVariable
    {
        public float Value { get; set; }
        public float InputSensitivity;

        public BinaryControlVariable(float inputSensitivity)
        {
            InputSensitivity = inputSensitivity;
        }

        public float FeedInput(bool positive, bool negative, float deltaTime)
        {
            return Value = BinaryControlStep(Value, positive, negative, InputSensitivity, deltaTime);
        }
    }

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

    private static float ToLogit(bool boolean)
    {
        return boolean ? 1 : 0;
    }
}