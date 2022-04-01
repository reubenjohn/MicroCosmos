using System;
using System.Collections.Generic;

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

        public static Tuple<T, float> ArgMin<T>(IEnumerable<T> list, Func<T, float> action)
        {
            var min = float.MaxValue;
            T argMin = default;
            foreach (var item in list)
            {
                var val = action.Invoke(item);
                if (val < min)
                {
                    min = val;
                    argMin = item;
                }
            }

            return new Tuple<T, float>(argMin, min);
        }
    }
}