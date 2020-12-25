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

        public static void RandomizeLogits(float[] arr, int length, int index = 0)
        {
            for (var i = index; i < length; i++)
                arr[i] = Random.Range(-1f, 1f);
        }

        public static float[,] RandomLogits(int length1, int length2)
        {
            var arr = new float[length1, length2];
            for (var i = 0; i < length1; i++)
            for (var j = 0; j < length2; j++)
                arr[i, j] = Random.Range(-1f, 1f);
            return arr;
        }

        public static void RandomizeLogits(float[,] arr, int length1, int length2, int index1 = 0, int index2 = 0)
        {
            for (var i = index1; i < length1; i++)
            for (var j = index2; j < length2; j++)
                arr[i, j] = Random.Range(-1f, 1f);
        }
    }
}