﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class KeyboardBrain : Brain, IBrain
{
    public float linearFlagellaSensitivity = 0.1f;
    public float angularFlagellaSensitivity = 0.1f;

    public new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public new void Update()
    {
        UpdateFlagellaLogits(actuatorLogits[0]);

        if (transform.TryGetComponent<Cell>(out Cell cell))
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                cell.GiveBirth();
            }
        }
        base.Update();
    }

    private void UpdateFlagellaLogits(float[] logits)
    {
        logits[0] = BinaryControlStep(logits[0], Input.GetKey(KeyCode.W), Input.GetKey(KeyCode.S), linearFlagellaSensitivity);
        logits[1] = BinaryControlStep(logits[1], Input.GetKey(KeyCode.A), Input.GetKey(KeyCode.D), angularFlagellaSensitivity);
    }

    private static float BinaryControlStep(float value, bool positiveInput, bool negativeInput, float inputSensitivity)
    {
        float linearTarget = ToLogit(positiveInput) - ToLogit(negativeInput);
        float remainingDistance = linearTarget - value;
        float maxLinearStep = Time.deltaTime * inputSensitivity;
        float linearStep = Mathf.Clamp(remainingDistance, -maxLinearStep, maxLinearStep);
        value += linearStep;
        return value;
    }

    private static float ToLogit(bool boolean)
    {
        return boolean ? 1 : 0;
    }
}
