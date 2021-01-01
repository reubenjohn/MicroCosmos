using System;
using System.Collections.Generic;
using System.Linq;
using Chemistry;
using ChemistryMicro;
using Environment;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Organelles.CellCauldron
{
    public class CellCauldronGene
    {
        public Dictionary<string, float> initialCauldron = new Dictionary<string, float>();
    }

    [RequireComponent(typeof(Cell.Cell), typeof(Rigidbody2D))]
    public partial class CellCauldron : PhysicalFlask, IActuator
    {
        private readonly RecipeBook recipeBook = RecipeBook.Singleton;
        private Cell.Cell cell;

        private CellCauldronGene gene;
        private ChemicalSink sink;
        public PhysicalFlask SourceFlask { get; set; }

        private void Start()
        {
            cell = GetComponent<Cell.Cell>();
            sink = GetComponentInParent<ChemicalSink>();
        }

        private void Update() => GrapherUtil.LogFlask(this, "Cauldron", 15, cell.IsInFocus);

        public string GetActuatorType() => typeof(CellCauldron).FullName;

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

            if (Time.frameCount % 10 == GetInstanceID() % 10)
            {
                var waste = this[Substance.Waste];
                if (waste > Mathf.Max(ChemicalBlob.MinBlobSize, TotalMass * .01f))
                    sink.Dump(transform.position, this,
                        new MixtureDictionary<Substance> {{Substance.Waste, waste}}.ToMixture());
            }
        }

        public void OnInheritGene(CellCauldronGene cellCauldronGene) => gene = cellCauldronGene;

        public CellCauldronGene GetGene() => gene;

        public void SetState(JObject jObject)
        {
            var chemicals = jObject?["chemicals"];
            if (chemicals != null)
            {
                var deserialized = chemicals.ToObject<Dictionary<string, float>>();
                var contents = EnumUtils.ParseNamedDictionary<Substance, float>(deserialized);
                var mixture = new Mixture<Substance>(contents);
                SourceFlask.TransferTo(this, mixture);
                SourceFlask = null;
                var initialMix = new Mixture<Substance>(
                    EnumUtils.ParseNamedDictionary<Substance, float>(gene.initialCauldron));
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

        public void DestroyConservatively()
        {
            sink.DumpAll(transform.position, this);
        }

        public static CellCauldronGene SampleGene()
        {
            var dic = new Dictionary<string, float>();
            foreach (var substanceName in Enum.GetNames(typeof(Substance)))
                dic[substanceName] = Random.Range(0f, .1f);
            dic[Substance.Fat.ToString()] = Random.Range(.5f, .8f);
            dic[Substance.Skin.ToString()] = Random.Range(.5f, .8f);
            dic[Substance.SkinGrowthFactor.ToString()] = Random.Range(.001f, .01f);

            var babyRelativeBirthMass = Mathf.Pow(10, Random.Range(-1f, 0f));
            var totalMass = dic.Sum(pair => pair.Value);
            var scaleFactor = babyRelativeBirthMass / totalMass;
            foreach (var pair in dic.ToArray())
                dic[pair.Key] = pair.Value * scaleFactor;

            return new CellCauldronGene {initialCauldron = dic};
        }
    }
}