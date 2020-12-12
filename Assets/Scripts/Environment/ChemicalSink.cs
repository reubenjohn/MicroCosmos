using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chemistry;
using ChemistryMicro;
using Newtonsoft.Json;
using UnityEngine;
using Util;

namespace Environment
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ChemicalSink : PhysicalFlask, ICellColonyListener
    {
        private Transform cellsTransform;
        private Transform inanimatesTransform;

        private void Start()
        {
            cellsTransform = transform.Find("CellColony");
            cellsTransform.GetComponent<CellColony>().AddListener(this);
            inanimatesTransform = transform.Find("Inanimate");
        }

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

        public void OnSave(string saveDirectory)
        {
            var serializer = new JsonSerializer {Formatting = Formatting.Indented};
            using (var sw = new StreamWriter(PersistenceFilePath(saveDirectory)))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, EnumUtils.ToNamedDictionary(flask.ToMixtureDictionary()));
            }
        }

        public void OnLoad(string saveDirectory)
        {
            var serializer = new JsonSerializer {Formatting = Formatting.Indented};
            using (var sr = new StreamReader(PersistenceFilePath(saveDirectory)))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                var namedDict = serializer.Deserialize<Dictionary<string, float>>(reader);
                var mixDict = EnumUtils.ParseNamedDictionary(namedDict, Substance.Waste);
                LoadFlask(mixDict);
            }
        }

        public void Dump(Vector3 dumpSite, PhysicalFlask source, Mixture<Substance> mix) =>
            ChemicalBlob.InstantiateBlob(source, mix, dumpSite, inanimatesTransform);

        private string PersistenceFilePath(string saveDir) => $"{saveDir}/chemicalSink1.json";
    }
}