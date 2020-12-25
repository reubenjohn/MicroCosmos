using System;
using System.Collections.Generic;
using System.Linq;

namespace Util
{
    public static class EnumUtils
    {
        public static Dictionary<TK, TV> ParseNamedDictionary<TK, TV>(Dictionary<string, TV> namedDict)
            where TK : struct
        {
            var enumDict = new Dictionary<TK, TV>();
            foreach (var pair in namedDict)
                if (Enum.TryParse<TK>(pair.Key, out var en))
                    enumDict[en] = pair.Value;
                else
                    throw new InvalidOperationException(
                        $"Could not parse '{pair.Key}' as {nameof(TK)} " +
                        $"while parsing named enum dictionary {namedDict}");

            return enumDict;
        }

        public static Dictionary<string, TV> ToNamedDictionary<TK, TV>(Dictionary<TK, TV> enumDict) where TK : struct
        {
            return enumDict.ToDictionary(
                pair => pair.Key.ToString(),
                pair => pair.Value
            );
        }

        public static int EnumCount(Type t) => Enum.GetValues(t).Length;
    }
}