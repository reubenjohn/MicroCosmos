using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Cell
{
    public class CellColony : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                var filePath = $"{Application.persistentDataPath}/save2.json";
                var serializer = new JsonSerializer {Formatting = Formatting.Indented};
                using (var sw = new StreamWriter(filePath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, Save());
                }

                Debug.Log("Saved to " + filePath);
            }
            else if (Input.GetKeyDown(KeyCode.F7))
            {
                var filePath = $"{Application.persistentDataPath}/save2.json";
                var serializer = new JsonSerializer {Formatting = Formatting.Indented};
                using (var sr = new StreamReader(filePath))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    Load(reader, serializer);
                }

                Debug.Log("Loaded from " + filePath);
            }
        }

        private CellColonyData Save()
        {
            return new CellColonyData {cells = GetCells().Select(CellData.Save).ToArray()};
        }

        private void Load(JsonReader reader, JsonSerializer serializer)
        {
            AssertToken(reader.Read() && reader.TokenType == JsonToken.StartObject);

            AssertToken(reader.Read() && reader.TokenType == JsonToken.PropertyName && (string) reader.Value == "cells");
            AssertToken(reader.Read() && reader.TokenType == JsonToken.StartArray);
            foreach (var child in transform.Children()) Destroy(child.gameObject);

            foreach (var cellData in LazyLoadCells(reader, serializer)) CellData.Load(cellData, transform);

            AssertToken(reader.TokenType == JsonToken.EndArray);

            AssertToken(reader.Read() && reader.TokenType == JsonToken.EndObject);
        }

        [AssertionMethod]
        private static void AssertToken(bool condition)
        {
            if (!condition) throw new Exception("Unexpected token");
        }

        private static IEnumerable<CellData> LazyLoadCells(JsonReader reader, JsonSerializer serializer)
        {
            while (reader.Read() && reader.TokenType == JsonToken.StartObject)
                yield return serializer.Deserialize<CellData>(reader);
        }

        public Cell[] GetCells()
        {
            return transform.GetComponentsInChildren<Cell>();
        }
    }

    public class CellColonyData
    {
        public CellData[] cells;
    }
}