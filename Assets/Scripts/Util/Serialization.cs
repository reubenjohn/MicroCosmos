using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Util
{
    public static class Serialization
    {
        public static string ToSerializable(Vector2 vec) => $"{vec.x} {vec.y}";

        public static Vector2 ToVector2(string vs)
        {
            var xy = vs.Split(' ');
            return new Vector2(float.Parse(xy[0]), float.Parse(xy[1]));
        }

        public static string ToPrintable(this float[] arr) => JsonConvert.SerializeObject(arr);

        public static string ToPrintable(this float[] arr, int precision) =>
            JsonConvert.SerializeObject(arr.Select(f => Math.Round(f, precision)));

        public static string ToPrintable(this float[][] arr, int precision) =>
            $"[{string.Join(",", arr.Select(a => a.ToPrintable(precision)))}]";

        public static string ReadAllCompressedText(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var decompressedStream = new GZipStream(fs, CompressionMode.Decompress, false))
            using (var sr = new StreamReader(decompressedStream))
            {
                return sr.ReadToEnd();
            }
        }
    }
}