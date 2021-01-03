using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Persistence;
using UnityEngine;

namespace Environment
{
    public class EnvironmentInitializer : MonoBehaviour
    {
        private void Start()
        {
            LoadMicroCosmosFromResources(
                new Dictionary<string, string>
                {
                    {"Environment.CellColony", $"Sample-{nameof(CellColony)}"},
                    {"Environment.ChemicalSink", $"Sample-{nameof(ChemicalSink)}"},
                    {"Environment.GenealogyGraphManager", $"Sample-{nameof(GenealogyGraphManager)}"},
                    {"Environment.ChemicalBlobPersistence", $"Sample-{nameof(ChemicalBlobPersistence)}"}
                }, out _
            );
        }

        public static void LoadMicroCosmosFromResources(
            Dictionary<string, string> savableResources,
            out Dictionary<string, string> resourceTexts
        )
        {
            using (var disposableDir = FabricateSaveDirectoryFromResources(savableResources, out resourceTexts))
            {
                var microCosmos = GameObject.Find("Environment").GetComponent<MicroCosmosPersistence>();
                var saveDirBackup = microCosmos.SaveDirectory;
                microCosmos.SaveDirectory = disposableDir.path;
                microCosmos.OnLoad();
                microCosmos.SaveDirectory = saveDirBackup;
            }
        }

        private static DisposableDirectory FabricateSaveDirectoryFromResources(
            Dictionary<string, string> savableResources, out Dictionary<string, string> resourceTexts)
        {
            var microCosmos = GameObject.Find("Environment").GetComponent<MicroCosmosPersistence>();
            resourceTexts = new Dictionary<string, string>();

            var disposableDir =
                new DisposableDirectory(
                    $"{Application.temporaryCachePath}/{nameof(EnvironmentInitializer)}/{Guid.NewGuid()}");
            foreach (var savable in microCosmos.SavableSubsystems)
            {
                var id = savable.GetID();
                if (savableResources.ContainsKey(id))
                    resourceTexts[id] = WriteToFile(
                        SubsystemsPersistence.GetSavePath(disposableDir.path, savable),
                        savableResources[id]);
            }

            return disposableDir;
        }

        private static string WriteToFile(string filePath, string resource)
        {
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var compressor = new GZipStream(fs, CompressionMode.Compress, false))
            using (var streamWriter = new StreamWriter(compressor))
            {
                var saveResource = Resources.Load<TextAsset>(resource);
                var saveResourceText = saveResource.text;
                streamWriter.Write(saveResourceText);
                return saveResourceText;
            }
        }

        private class DisposableDirectory : IDisposable
        {
            public readonly string path;

            public DisposableDirectory(string path)
            {
                this.path = path;
                Directory.CreateDirectory(path);
            }

            public void Dispose() => Directory.Delete(path, true);
        }
    }
}