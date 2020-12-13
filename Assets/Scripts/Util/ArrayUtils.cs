namespace Util
{
    public static class ArrayUtils
    {
        public static void Copy(float[,] source, float[,] destination, int length1, int length2)
        {
            for (var i = 0; i < length1; i++)
            for (var j = 0; j < length2; j++)
                destination[i, j] = source[i, j];
        }
    }
}