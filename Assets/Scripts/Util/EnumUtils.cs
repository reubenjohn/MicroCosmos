using System;
using System.Collections.Generic;
using System.Linq;

namespace Util
{
    public static class EnumUtils
    {
        public static Dictionary<TK, TV> ParseNamedDictionary<TK, TV>(Dictionary<string, TV> enumDict, TK defaultVal)
            where TK : struct
        {
            return enumDict.ToDictionary(
                pair => Enum.TryParse<TK>(pair.Key, out var substance)
                    ? substance
                    : defaultVal,
                pair => pair.Value
            );
        }

        public static Dictionary<string, TV> ToNamedDictionary<TK, TV>(Dictionary<TK, TV> enumDict) where TK : struct
        {
            return enumDict.ToDictionary(
                pair => pair.Key.ToString(),
                pair => pair.Value
            );
        }
    }
}