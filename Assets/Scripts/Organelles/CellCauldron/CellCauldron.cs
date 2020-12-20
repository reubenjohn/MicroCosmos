using System;
using System.Collections.Generic;
using Chemistry;
using ChemistryMicro;
using Environment;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

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
        public PhysicalFlask SourceFlask { get; set; }

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

        public void OnInheritGene(ChemicalBagGene chemicalBagGene) => gene = chemicalBagGene;

        public ChemicalBagGene GetGene() => gene;

        public void SetState(JObject jObject)
        {
            var chemicals = jObject?["chemicals"];
            if (chemicals != null)
            {
                var deserialized = chemicals.ToObject<Dictionary<string, float>>();
                var contents = EnumUtils.ParseNamedDictionary(deserialized, Substance.Waste);
                var mixture = new Mixture<Substance>(contents);
                SourceFlask.TransferTo(this, mixture);
                SourceFlask = null;
                var initialMix = new Mixture<Substance>(
                    EnumUtils.ParseNamedDictionary(gene.initialCauldron, Substance.Waste));
                var convertBabyFat = (bool) (jObject["convertBabyFat"] ?? true);
                if (convertBabyFat)
                {
                    var reaction = new Reaction<Substance>(
                        new MixtureDictionary<Substance> {{Substance.Fat, initialMix.TotalMass}}.ToMixture(),
                        initialMix
                    );
                    Convert(reaction);
                }
            }
        }

        public JObject GetState() => GetState(ToMixture(), false);

        public static JObject GetState(Mixture<Substance> mixture, bool convertBabyFat) =>
            new JObject
            {
                ["chemicals"] = JObject.FromObject(EnumUtils.ToNamedDictionary(mixture.ToMixtureDictionary())),
                ["convertBabyFat"] = convertBabyFat
            };

        public void OnDying() => sink.Dump(transform.position, this, ToMixture());

        public static ChemicalBagGene SampleGene()
        {
            var dic = new Dictionary<string, float>();
            foreach (var substanceName in Enum.GetNames(typeof(Substance)))
                dic[substanceName] = Random.Range(0f, .1f);
            dic[Substance.Fat.ToString()] = Random.Range(.5f, .8f);
            dic[Substance.Skin.ToString()] = Random.Range(.5f, .8f);
            dic[Substance.SkinGrowthFactor.ToString()] = Random.Range(.001f, .01f);

            return new ChemicalBagGene
            {
                initialCauldron = dic
            };
        }
    }
}