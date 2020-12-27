using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Persistence;
using UnityEngine;

namespace Environment
{
    public class ChemicalBlobPersistence : MonoBehaviour, ISavableSubsystem<ChemicalBlobSaveItem>
    {
        private ChemicalSink sink;

        private void Start() => sink = GetComponentInParent<ChemicalSink>();

        public string GetID() => typeof(ChemicalBlobPersistence).FullName;

        public int GetPersistenceVersion() => 1;

        public Type GetSavableType() => typeof(ChemicalBlobSaveItem);

        IEnumerable ISavableSubsystem.Save() => Save();

        public IEnumerable<ChemicalBlobSaveItem> Save()
        {
            foreach (var blob in GetComponentsInChildren<ChemicalBlob>())
                yield return ChemicalBlob.Save(blob);
        }

        public void Load(IEnumerable save) => Load(save.Cast<ChemicalBlobSaveItem>());

        public void Load(IEnumerable<ChemicalBlobSaveItem> save)
        {
            foreach (var blob in GetComponentsInChildren<ChemicalBlob>())
            {
                Destroy(blob.gameObject);
                sink.Recover(blob);
            }

            foreach (var item in save)
                if (item is ChemicalBlobSave blobSave)
                    ChemicalBlob.Load(sink, blobSave, transform);
                else throw new InvalidOperationException($"Unsupported save item of type '{nameof(item)}'");
        }

        public JsonSerializer GetSerializer() =>
            new JsonSerializer
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };
    }

    public abstract class ChemicalBlobSaveItem { }

    public class ChemicalBlobSave : ChemicalBlobSaveItem
    {
        public ChemicalBlobSave(string position, Dictionary<string, float> mixture)
        {
            Position = position;
            Mixture = mixture;
        }

        public string Position { get; }
        public Dictionary<string, float> Mixture { get; }
    }
}