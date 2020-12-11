using System.Collections.Generic;
using Chemistry;
using ChemistryMicro;
using Environment;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Organelles.CellCauldron
{
    public class ChemicalBagGene
    {
        public Dictionary<string, float> initialCauldron = new Dictionary<string, float>();
    }

    [RequireComponent(typeof(Cell.Cell), typeof(Rigidbody2D))]
    public partial class CellCauldron : MonoBehaviour, IActuator
    {
        private readonly RecipeBook recipeBook = RecipeBook.Singleton;
        private Cell.Cell cell;
        private Flask<Substance> flask;

        private ChemicalBagGene gene;
        private Rigidbody2D rb;
        private ChemicalSink sink;
        public float TotalMass => flask.TotalMass;

        public float this[Substance substance] => flask[substance];

        private void Start()
        {
            cell = GetComponent<Cell.Cell>();
            rb = GetComponent<Rigidbody2D>();
            sink = GetComponentInParent<ChemicalSink>();
        }

        private void Update()
        {
            GrapherUtil.LogFlask(flask, "Cauldron", 15, cell.IsInFocus);
            if (Time.frameCount % 15 == 0)
                rb.mass = TotalMass;
        }

        public float[] Connect() => new float[VoluntaryRecipes.Length];

        public void Actuate(float[] logits)
        {
            for (var i = 0; i < VoluntaryRecipes.Length; i++)
            {
                var recipe = VoluntaryRecipes[i];
                if (recipe == Recipe.Nop) continue;

                flask.Convert(recipeBook[recipe], Mathf.Max(0, logits[i]));
            }

            foreach (var recipe in InvoluntaryRecipes)
                flask.Convert(recipeBook[recipe]);
            var waste = flask[Substance.Waste];
            if (waste > 0)
                sink.Dump(transform.position, flask,
                    new MixtureDictionary<Substance> {{Substance.Waste, waste}}.ToMixture());
        }

        public void OnInheritGene(ChemicalBagGene chemicalBagGene)
        {
            gene = chemicalBagGene;

            var parsedDict = EnumUtils.ParseNamedDictionary(gene.initialCauldron, Substance.Fat);
            flask = new Flask<Substance>(parsedDict);
        }

        public ChemicalBagGene GetGene() => gene;

        public void SetState(JObject jObject)
        {
            var chemicals = jObject?["chemicals"];
            if (chemicals != null)
            {
                var deserialized = chemicals.ToObject<Dictionary<string, float>>();
                var initialMix = EnumUtils.ParseNamedDictionary(deserialized, Substance.Waste);
                flask = new Flask<Substance>(initialMix);
            }
        }

        public JObject GetState() =>
            new JObject
            {
                ["chemicals"] = JToken.FromObject(EnumUtils.ToNamedDictionary(flask.ToMixtureDictionary()))
            };

        public void Burst()
        {
            sink.Dump(transform.position, flask, flask);
        }
    }
}