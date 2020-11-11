namespace Brains
{
    public static class Logits
    {
        public static void Flatten(float[][] inputs, float[] flattenedOutput)
        {
            var inputFullyCompleted = 0;
            foreach (var input in inputs)
            {
                input.CopyTo(flattenedOutput, inputFullyCompleted);
                inputFullyCompleted += input.Length;
            }
        }

        public static void Unflatten(float[] flattenedLogits, float[][] unflattenedLogits)
        {
            var inputsFullyCompleted = 0;
            foreach (var output in unflattenedLogits)
            {
                for (var i = 0; i < output.Length; i++)
                {
                    output[i] = flattenedLogits[inputsFullyCompleted + i];
                }

                inputsFullyCompleted += output.Length;
            }
        }
    }
}