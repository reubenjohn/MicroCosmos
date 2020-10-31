using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

public class CellColony : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            string filePath = Application.persistentDataPath + "/save2.json";
            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            using (StreamWriter sw = new StreamWriter(filePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, this.Save());
            }
            UnityEngine.Debug.Log("Saved to " + filePath);
        }
        else if (Input.GetKeyDown(KeyCode.F7))
        {
            string filePath = Application.persistentDataPath + "/save2.json";
            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            using (StreamReader sr = new StreamReader(filePath))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                Load(reader, serializer);
            }
            UnityEngine.Debug.Log("Loaded from " + filePath);
        }
    }

    private CellColonyData Save()
    {
        return new CellColonyData() { cells = GetCells().Select(cell => CellData.Save(cell)) };
    }

    private void Load(JsonReader reader, JsonSerializer serializer)
    {
        AssertToken(reader.Read() && reader.TokenType == JsonToken.StartObject);

        AssertToken(reader.Read() && reader.TokenType == JsonToken.PropertyName && (string)reader.Value == "cells");
        AssertToken(reader.Read() && reader.TokenType == JsonToken.StartArray);
        foreach (var child in transform.Children())
        {
            Destroy(child.gameObject);
        }
        foreach (var cellData in LazyLoadCells(reader, serializer))
        {
            CellData.Load(cellData, transform);
        }
        AssertToken(reader.TokenType == JsonToken.EndArray);

        AssertToken(reader.Read() && reader.TokenType == JsonToken.EndObject);
    }

    void AssertToken(bool condition)
    {
        if (!condition)
        {
            throw new Exception("Unexpected token");
        }
    }

    private IEnumerable<CellData> LazyLoadCells(JsonReader reader, JsonSerializer serializer)
    {
        while (reader.Read() && reader.TokenType == JsonToken.StartObject)
        {
            yield return serializer.Deserialize<CellData>(reader);
        }
    }

    public Cell[] GetCells()
    {
        return transform.GetComponentsInChildren<Cell>();
    }
}

public class CellColonyData
{
    public IEnumerable<CellData> cells;
}
