using UnityEngine;

namespace Util
{
    public static class RandomUtils
    {
        public static float[] RandomLogits(int length)
        {
            var arr = new float[length];
            for (var i = 0; i < length; i++)
                arr[i] = Random.Range(-1f, 1f);
            return arr;
        }

        public static float[,] RandomLogits(int length1, int length2)
        {
            var arr = new float[length1, length2];
            for (var i = 0; i < length1; i++)
            for (var j = 0; j < length2; j++)
                arr[i, j] = Random.Range(-1f, 1f);
            return arr;
        }
    }
}