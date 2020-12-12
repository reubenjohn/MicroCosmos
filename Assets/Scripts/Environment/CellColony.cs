using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cell;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using Util;

namespace Environment
{
    public class CellColony : MonoBehaviour
    {
        public string saveFile = "save2";
        private readonly List<ICellColonyListener> listeners = new List<ICellColonyListener>();
        private string saveDirectory;

        public string SaveDirectory
        {
            get => saveDirectory = string.IsNullOrEmpty(saveDirectory) ? DefaultDirectory : saveDirectory;
            set => saveDirectory = value;
        }

        public string SavePath => $"{SaveDirectory}/{saveFile}.json";

        private string DefaultDirectory => $"{Application.persistentDataPath}/saves";

        private Cell.Cell[] LivingCells => transform.GetComponentsInChildren<Cell.Cell>();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6)) OnSave();
            else if (Input.GetKeyDown(KeyCode.F7)) OnLoad();
        }

        public void OnSave()
        {
            Directory.CreateDirectory(SaveDirectory);

            foreach (var listener in listeners)
                try
                {
                    listener.OnSave(SaveDirectory);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Could not save for listener: {e.Message}\n{e.StackTrace}");
                }

            var serializer = new JsonSerializer {Formatting = Formatting.Indented};
            using (var sw = new StreamWriter(SavePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, SaveCellData());
            }

            Debug.Log($"Saved cell colony to {SavePath}");
        }

        public void OnLoad()
        {
            foreach (var listener in listeners) listener.OnLoad(SaveDirectory);

            var serializer = new JsonSerializer {Formatting = Formatting.Indented};
            using (var sr = new StreamReader(SavePath))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                Load(reader, serializer);
            }

            Debug.Log($"Loaded cell colony from {SavePath}");
        }

        private CellColonyData SaveCellData() =>
            new CellColonyData {cells = LivingCells.Select(CellData.Save).ToArray()};

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

        public Cell.Cell FindCell(Guid genealogyNodeGuid)
        {
            return LivingCells
                .FirstOrDefault(c => genealogyNodeGuid == c.GenealogyNode.Guid);
        }

        public void AddListener(ICellColonyListener listener)
        {
            listeners.Add(listener);
        }
    }

    public class CellColonyData
    {
        public CellData[] cells;
    }
}