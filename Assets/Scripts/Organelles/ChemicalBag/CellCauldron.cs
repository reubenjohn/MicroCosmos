using System.Collections.Generic;
using Cell.Colony;
using Chemistry;
using ChemistryMicro;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Organelles.ChemicalBag
{
    public class ChemicalBagGene
    {
        public Dictionary<string, float> initialCauldron = new Dictionary<string, float>();
    }

    [RequireComponent(typeof(Cell.Cell))]
    public class CellCauldron : MonoBehaviour, IActuator
    {
        private static readonly Recipe[] InvoluntaryRecipes = {Recipe.AgeSkin};

        private readonly RecipeBook recipeBook = RecipeBook.Singleton;
        private Cell.Cell cell;
        private Flask<Substance> flask;

        private ChemicalBagGene gene;
        private ChemicalSink sink;
        public float TotalMass => flask.TotalMass;

        private void Start()
        {
            cell = GetComponent<Cell.Cell>();
            sink = GetComponentInParent<ChemicalSink>();
        }

        private void Update()
        {
            GrapherUtil.LogFlask(flask, "Cauldron", 15, cell.IsInFocus);
        }

        public float[] Connect()
        {
            return new float[recipeBook.voluntaryRecipes.Length];
        }

        public void Actuate(float[] logits)
        {
            for (var i = 0; i < recipeBook.voluntaryRecipes.Length; i++)
            {
                var recipe = recipeBook.voluntaryRecipes[i];
                if (recipe == Recipe.NOP) continue;

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

        public ChemicalBagGene GetGene()
        {
            return gene;
        }

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

        public JObject GetState()
        {
            return new JObject
            {
                ["chemicals"] = JToken.FromObject(EnumUtils.ToNamedDictionary(flask.ToMixtureDictionary()))
            };
        }
    }
}