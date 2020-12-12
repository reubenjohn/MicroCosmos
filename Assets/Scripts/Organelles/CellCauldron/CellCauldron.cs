using System.Collections.Generic;
using Chemistry;
using ChemistryMicro;
using Environment;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Util;

namespace Organelles.CellCauldron
{
    public class ChemicalBagGene
    {
        public Dictionary<string, float> initialCauldron = new Dictionary<string, float>();
    }

    [RequireComponent(typeof(Cell.Cell), typeof(Rigidbody2D))]
    public partial class CellCauldron : PhysicalFlask, IActuator
    {
        private readonly RecipeBook recipeBook = RecipeBook.Singleton;
        private Cell.Cell cell;

        private ChemicalBagGene gene;
        private ChemicalSink sink;

        private void Start()
        {
            cell = GetComponent<Cell.Cell>();
            sink = GetComponentInParent<ChemicalSink>();
        }

        private void Update() => GrapherUtil.LogFlask(this, "Cauldron", 15, cell.IsInFocus);

        public float[] Connect() => new float[VoluntaryRecipes.Length];

        public void Actuate(float[] logits)
        {
            for (var i = 0; i < VoluntaryRecipes.Length; i++)
            {
                var recipe = VoluntaryRecipes[i];
                if (recipe == Recipe.Nop) continue;

                Convert(recipeBook[recipe], Mathf.Max(0, logits[i]));
            }

            foreach (var recipe in InvoluntaryRecipes)
                Convert(recipeBook[recipe]);

            var waste = this[Substance.Waste];
            if (waste > ChemicalBlob.MinBlobSize)
                sink.Dump(transform.position, this,
                    new MixtureDictionary<Substance> {{Substance.Waste, waste}}.ToMixture());
        }

        public void OnInheritGene(ChemicalBagGene chemicalBagGene)
        {
            gene = chemicalBagGene;

            var parsedDict = EnumUtils.ParseNamedDictionary(gene.initialCauldron, Substance.Fat);
            LoadFlask(parsedDict);
        }

        public ChemicalBagGene GetGene() => gene;

        public void SetState(JObject jObject)
        {
            var chemicals = jObject?["chemicals"];
            if (chemicals != null)
            {
                var deserialized = chemicals.ToObject<Dictionary<string, float>>();
                var initialMix = EnumUtils.ParseNamedDictionary(deserialized, Substance.Waste);
                LoadFlask(initialMix);
            }
        }

        public JObject GetState() =>
            new JObject
            {
                ["chemicals"] = JToken.FromObject(EnumUtils.ToNamedDictionary(ToMixture().ToMixtureDictionary()))
            };

        public void OnDying() => sink.Dump(transform.position, this, ToMixture());
    }
}