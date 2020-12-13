using System;
using UnityEngine;

namespace Brains
{
    public static class Logits
    {
        public static void Clamp(float[] flattenedOutput)
        {
            for (var i = 0; i < flattenedOutput.Length; i++)
                flattenedOutput[i] = Mathf.Clamp(flattenedOutput[i], -1, 1);
        }

        public static void Flatten(float[][] inputs, float[] flattenedOutput, bool failIfOutputTooLong = true)
        {
            var inputFullyCompleted = 0;
            foreach (var input in inputs)
            {
                input.CopyTo(flattenedOutput, inputFullyCompleted);
                inputFullyCompleted += input.Length;
            }

            if (failIfOutputTooLong && inputFullyCompleted < flattenedOutput.Length)
                throw new ArgumentException("Destination array was too long");
        }

        public static void Unflatten(float[] flattenedLogits, float[][] unflattenedLogits)
        {
            var inputsFullyCompleted = 0;
            foreach (var output in unflattenedLogits)
            {
                for (var i = 0; i < output.Length; i++)
                    output[i] = flattenedLogits[inputsFullyCompleted + i];
                inputsFullyCompleted += output.Length;
            }
        }
    }
}