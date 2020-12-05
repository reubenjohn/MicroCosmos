using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chemistry;
using ChemistryMicro;
using Newtonsoft.Json;
using Organelles.ChemicalBag;
using UnityEngine;

namespace Cell.Colony
{
    public class ChemicalSink : MonoBehaviour, ICellColonyListener
    {
        private Flask<Substance> flask;

        private void Start()
        {
            GetComponent<CellColony>().AddListener(this);
            flask = new Flask<Substance>();
        }

        private void Update()
        {
            GrapherUtil.LogFlask(flask, "ChemicalSink", 30);

            if (Time.frameCount % 30 == 0)
            {
                var totalMass = flask.TotalMass +
                                GetComponentsInChildren<CellCauldron>()
                                    .Sum(cauldron => cauldron.TotalMass);
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
                flask = new Flask<Substance>(mixDict);
            }
        }

        public void Dump(Vector3 dumpSite, Flask<Substance> source, Mixture<Substance> mix)
        {
            //TODO Manifest as blob at dump site
            Flask<Substance>.Transfer(flask, source, mix);
        }

        private string PersistenceFilePath(string saveDir)
        {
            return $"{saveDir}/chemicalSink1.json";
        }
    }
}