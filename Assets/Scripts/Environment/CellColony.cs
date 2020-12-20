using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cell;
using ChemistryMicro;
using Genetics;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Organelles.CellCauldron;
using Persistence;
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

        private void Start() => GetComponent<GenealogyGraphManager>();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6)) OnSave();
            else if (Input.GetKeyDown(KeyCode.F7)) OnLoad();
        }

        public void OnSave()
        {
            Debug.Log($"Saving cell colony to {SavePath}");

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
        }

        public void OnLoad()
        {
            Debug.Log($"Loading cell colony from {SavePath}");

            foreach (var listener in listeners) listener.OnLoad(SaveDirectory);

            var serializer = new JsonSerializer {Formatting = Formatting.Indented};
            using (var sr = new StreamReader(SavePath))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                Load(reader, serializer);
            }
        }

        private CellColonyData SaveCellData() => new CellColonyData {cells = LivingCells.Select(ToCellData).ToArray()};

        private CellData ToCellData(Cell.Cell cell) =>
            new CellData
            {
                geneTree = GeneNode.Save(cell),
                stateTree = StateNode.Save(cell)
            };

        private void Load(JsonReader reader, JsonSerializer serializer)
        {
            AssertToken(reader.Read() && reader.TokenType == JsonToken.StartObject);

            AssertToken(reader.Read() && reader.TokenType == JsonToken.PropertyName &&
                        (string) reader.Value == "cells");
            AssertToken(reader.Read() && reader.TokenType == JsonToken.StartArray);
            foreach (var child in transform.Children()) Destroy(child.gameObject);

            foreach (var cellData in LazyLoadCells(reader, serializer))
                SpawnCell(cellData, GetComponentInParent<ChemicalSink>());

            AssertToken(reader.TokenType == JsonToken.EndArray);

            AssertToken(reader.Read() && reader.TokenType == JsonToken.EndObject);
        }

        public GameObject SpawnCell(GeneNode geneTree, JObject cellState, PhysicalFlask sourceFlask)
        {
            var stateTree = StateNode.Empty(geneTree);
            stateTree.state = cellState;
            return SpawnCell(new CellData {geneTree = geneTree, stateTree = stateTree}, sourceFlask);
        }

        private GameObject SpawnCell(CellData cellData, PhysicalFlask sourceFlask)
        {
            var cellObj = GeneNode.Load(cellData.geneTree, transform);
            cellObj.GetComponent<CellCauldron>().SourceFlask = sourceFlask;
            StateNode.Load(cellObj.GetComponent<ILivingComponent>(), cellData.stateTree);
            return cellObj;
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