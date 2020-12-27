using System;
using System.Collections;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Persistence
{
    public class SubsystemsPersistence
    {
        private readonly ISavableSubsystem[] savableSubsystems;

        public SubsystemsPersistence(ISavableSubsystem[] savableSubsystems, string saveDirectory)
        {
            this.savableSubsystems = savableSubsystems;
            SaveDirectory = saveDirectory;
        }

        public string SaveDirectory { get; set; }

        public static string GetSavePath(string saveDirectory, ISavableSubsystem savable) =>
            $"{saveDirectory}/{savable.GetID()}-{savable.GetPersistenceVersion()}.json";

        private string GetSavePath(ISavableSubsystem savable) => GetSavePath(SaveDirectory, savable);

        public void Save()
        {
            Debug.Log($"Saving simulation to '{SaveDirectory}/'");

            Directory.CreateDirectory(SaveDirectory);

            foreach (var savable in savableSubsystems)
                try
                {
                    var savePath = GetSavePath(savable);
                    Debug.Log($"Saving savable subsystem '{savable.GetID()}' to {savePath}");
                    using (var sw = new StreamWriter(savePath))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        savable.GetSerializer().Serialize(writer, savable.Save());
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Could not save subsystem '{savable.GetID()}': {e.Message}\n{e.StackTrace}");
                }
        }

        public void Load()
        {
            Debug.Log($"Loading simulation from {SaveDirectory}");

            foreach (var savable in savableSubsystems)
                try
                {
                    var savePath = GetSavePath(savable);
                    Debug.Log($"Loading savable subsystem '{savable.GetID()}' to {savePath}");
                    using (var sr = new StreamReader(savePath))
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        savable.Load(LoadEnumerable(reader, savable.GetSerializer(), savable.GetSavableType()));
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Could not save subsystem '{savable.GetID()}': {e.Message}\n{e.StackTrace}");
                }
        }

        private static IEnumerable LoadEnumerable(JsonReader reader, JsonSerializer serializer, Type type)
        {
            AssertToken(reader.Read() && reader.TokenType == JsonToken.StartArray);

            while (reader.Read() && reader.TokenType == JsonToken.StartObject)
                yield return serializer.Deserialize(reader, type);

            AssertToken(reader.TokenType == JsonToken.EndArray);
        }

        [AssertionMethod]
        private static void AssertToken(bool condition)
        {
            if (!condition) throw new Exception("Unexpected token");
        }
    }
}