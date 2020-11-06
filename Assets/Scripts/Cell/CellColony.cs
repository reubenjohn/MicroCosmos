using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cell
{
    public class CellColony : MonoBehaviour
    {
        public string saveFile;

        private void Start()
        {
            saveFile = saveFile ?? $"{Application.persistentDataPath}/save2.json";
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6))
                OnSave();
            else if (Input.GetKeyDown(KeyCode.F7)) OnLoad();
        }

        public void OnSave()
        {
            var serializer = new JsonSerializer {Formatting = Formatting.Indented};
            using (var sw = new StreamWriter(saveFile))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, SaveCellData());
            }

            Debug.Log($"Saved cell colony to {saveFile}");
        }

        public void OnLoad()
        {
            var serializer = new JsonSerializer {Formatting = Formatting.Indented};
            using (var sr = new StreamReader(saveFile))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                Load(reader, serializer);
            }

            Debug.Log($"Loaded cell colony from {saveFile}");
        }

        private CellColonyData SaveCellData() =>
            new CellColonyData {cells = GetCells().Select(CellData.Save).ToArray()};

        private void Load(JsonReader reader, JsonSerializer serializer)
        {
            AssertToken(reader.Read() && reader.TokenType == JsonToken.StartObject);

            AssertToken(reader.Read() && reader.TokenType == JsonToken.PropertyName &&
                        (string) reader.Value == "cells");
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

        private Cell[] GetCells()
        {
            return transform.GetComponentsInChildren<Cell>();
        }
    }

    public class CellColonyData
    {
        public CellData[] cells;
    }
}