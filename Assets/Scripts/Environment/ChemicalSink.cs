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

        private void Start() => inanimatesTransform = transform.Find("Inanimate");

        private void Update()
        {
            GrapherUtil.LogFlask(this, "ChemicalSink", 30);

            if (Time.frameCount % 30 == 0)
            {
                var totalMass = TotalMass +
                                transform.GetComponentsInChildren<IFlaskBehavior<Substance>>()
                                    .Sum(flaskBehavior => flaskBehavior.TotalMass);
                Grapher.Log(totalMass, "TotalMass");
            }
        }

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

        private void LoadFlask(Dictionary<Substance, float> newFlaskContents)
        {
            var source = new Flask<Substance>(newFlaskContents);
            Flask<Substance>.Transfer(flask, source, source - flask);
            OnReflectPhysicalProperties();
            if (source.TotalMass > 0)
                Debug.LogWarning($"'{gameObject.name}' destroying {source.TotalMass}kg while loading");
        }

        public void Dump(Vector3 dumpSite, PhysicalFlask source, Mixture<Substance> mix) =>
            ChemicalBlob.InstantiateBlob(source, mix, dumpSite, inanimatesTransform);

        // TODO Instead of override, create a base class that does not reflect physical properties
        protected override void OnReflectPhysicalProperties() { }
    }

    public abstract class ChemicalSinkSaveItem { }

    public class ChemicalSinkChemicals : ChemicalSinkSaveItem
    {
        public Dictionary<string, float> mixture;
    }
}