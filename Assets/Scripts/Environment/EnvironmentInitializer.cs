using System.Collections.Generic;
using System.IO;
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
            CreateSaveDirectoryFromResources(savableResources, out resourceTexts);
            var microCosmos = GameObject.Find("Environment").GetComponent<MicroCosmosPersistence>();
            microCosmos.OnLoad();
        }

        private static void CreateSaveDirectoryFromResources(
            Dictionary<string, string> savableResources, out Dictionary<string, string> resourceTexts)
        {
            var microCosmos = GameObject.Find("Environment").GetComponent<MicroCosmosPersistence>();
            resourceTexts = new Dictionary<string, string>();

            Directory.CreateDirectory(microCosmos.SaveDirectory);
            foreach (var savable in microCosmos.SavableSubsystems)
            {
                var id = savable.GetID();
                if (savableResources.ContainsKey(id))
                    resourceTexts[id] = WriteToFile(
                        SubsystemsPersistence.GetSavePath(microCosmos.SaveDirectory, savable),
                        savableResources[id]);
            }
        }

        private static string WriteToFile(string filePath, string resource)
        {
            using (var streamWriter = File.CreateText(filePath))
            {
                var saveResource = Resources.Load<TextAsset>(resource);
                var saveResourceText = saveResource.text;
                streamWriter.Write(saveResourceText);
                return saveResourceText;
            }
        }
    }
}