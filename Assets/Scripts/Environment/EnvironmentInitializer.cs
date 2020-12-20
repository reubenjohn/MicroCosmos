using System;
using System.IO;
using UnityEngine;

namespace Environment
{
    public class EnvironmentInitializer : MonoBehaviour
    {
        private void Start()
        {
            LoadEnvironmentFromResources(
                "Sample-CellColonySave", out _,
                "Sample-ChemicalSinkSave", out _,
                "Sample-GenealogyScrollSave", out _
            );
        }

        public static void LoadEnvironmentFromResources(
            string cellColonyResource, out string cellColonyResourceText,
            string chemicalSinkResource, out string chemicalSinkResourceText,
            string genealogyScrollResource, out string genealogyScrollResourceText
        )
        {
            var cellColony = GameObject.Find("CellColony").GetComponent<CellColony>();
            var graphManager = GameObject.Find("CellColony").GetComponent<GenealogyGraphManager>();
            var chemicalSink = GameObject.Find("Environment").GetComponent<ChemicalSink>();

            var saveDirBackup = cellColony.SaveDirectory;
            cellColony.SaveDirectory = $"{Application.temporaryCachePath}/testing/{Guid.NewGuid()}";

            var loadFile = cellColony.SavePath;
            var loadFileChemicalSink = chemicalSink.PersistenceFilePath(cellColony.SaveDirectory);
            var loadFileGenealogy = graphManager.PersistenceFilePath(cellColony.SaveDirectory);

            Directory.CreateDirectory(cellColony.SaveDirectory);

            cellColonyResourceText = WriteToFile(loadFile, cellColonyResource);
            chemicalSinkResourceText = WriteToFile(loadFileChemicalSink, chemicalSinkResource);
            genealogyScrollResourceText = WriteToFile(loadFileGenealogy, genealogyScrollResource);

            cellColony.OnLoad();
            Directory.Delete(cellColony.SaveDirectory, true);
            cellColony.SaveDirectory = saveDirBackup;
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