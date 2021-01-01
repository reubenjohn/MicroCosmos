using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chemistry;
using ChemistryMicro;
using Newtonsoft.Json;
using Persistence;
using UnityEngine;
using Util;

namespace Environment
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ChemicalSink : PhysicalFlask, ISavableSubsystem<ChemicalSinkSaveItem>
    {
        private Transform inanimatesTransform;

        private void Start()
        {
            inanimatesTransform = transform.Find("Inanimate");
            StartCoroutine(StartStatsPlotting());
        }

        protected override void OnDestroy() { } // Don't log mass conservation warning

        public string GetID() => typeof(ChemicalSink).FullName;

        public int GetPersistenceVersion() => 1;

        public Type GetSavableType() => typeof(ChemicalSinkSaveItem);

        public JsonSerializer GetSerializer() =>
            new JsonSerializer
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };

        IEnumerable ISavableSubsystem.Save() => Save();

        public IEnumerable<ChemicalSinkSaveItem> Save() =>
            new ChemicalSinkSaveItem[]
            {
                new ChemicalSinkChemicals
                {
                    mixture = EnumUtils.ToNamedDictionary(flask.ToMixtureDictionary())
                }
            };

        public void Load(IEnumerable save) => Load(save.Cast<ChemicalSinkSaveItem>());

        public void Load(IEnumerable<ChemicalSinkSaveItem> save)
        {
            foreach (var item in save)
                if (item is ChemicalSinkChemicals chemicalsItem)
                    LoadFlask(EnumUtils.ParseNamedDictionary<Substance, float>(chemicalsItem.mixture));
                else
                    throw new InvalidDataException($"Cannot load item of type '{save.GetType().FullName}'");
        }

        private IEnumerator StartStatsPlotting()
        {
            while (true)
            {
                // GrapherUtil.LogFlask(this, "ChemicalSink", 30);
                var flasks = transform.GetComponentsInChildren<IFlaskBehavior<Substance>>();
                var totalMass = TotalMass +
                                flasks
                                    .Sum(flaskBehavior => flaskBehavior.TotalMass);
                Grapher.Log(totalMass, "TotalMass");
                yield return new WaitForSeconds(3);
            }

            // ReSharper disable once IteratorNeverReturns
        }

        private void LoadFlask(Dictionary<Substance, float> newFlaskContents)
        {
            var source = new Flask<Substance>(newFlaskContents);
            Flask<Substance>.Transfer(flask, source, source - flask);
            OnReflectPhysicalProperties();
            if (source.TotalMass > 0)
                Debug.LogWarning($"'{gameObject.name}' destroying {source.TotalMass}kg while loading");
        }

        public void Dump(Vector3 dumpSite, PhysicalFlask source, Mixture<Substance> mix)
        {
            var mass = mix.TotalMass;
            if (mass != 0.0)
            {
                if (mass > ChemicalBlob.MinBlobSize)
                    ChemicalBlob.InstantiateBlob(source, mix, dumpSite, inanimatesTransform);
                else
                    Debug.LogWarning(
                        $"Dumping mix '{mix}' with mass ({mass}) < MinBlobSize ({ChemicalBlob.MinBlobSize})");
            }
        }

        public void DumpAll(Vector3 dumpSite, PhysicalFlask source)
        {
            var mass = source.TotalMass;
            if (mass != 0.0)
            {
                if (mass > ChemicalBlob.MinBlobSize)
                    ChemicalBlob.InstantiateBlob(source, dumpSite, inanimatesTransform);
                else
                    Debug.LogWarning(
                        $"Dumping mix '{source.ToMixture()}' with mass ({mass}) < MinBlobSize ({ChemicalBlob.MinBlobSize})");
            }
        }


        // TODO Instead of override, create a base class that does not reflect physical properties
        protected override void OnReflectPhysicalProperties() { }
    }

    public abstract class ChemicalSinkSaveItem { }

    public class ChemicalSinkChemicals : ChemicalSinkSaveItem
    {
        public Dictionary<string, float> mixture;
    }
}