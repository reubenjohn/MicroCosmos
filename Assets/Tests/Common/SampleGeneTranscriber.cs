using System;
using System.Linq;
using Genetics;
using Newtonsoft.Json.Linq;
using Random = UnityEngine.Random;

namespace Tests.Common
{
    public class SampleGeneTranscriber : GeneTranscriber<SampleGene>
    {
        public static readonly SampleGeneTranscriber Singleton = new SampleGeneTranscriber();
        private static readonly Mutator.ClampedFloat FurrinessMutator = new Mutator.ClampedFloat(0.1f, 0f, 1f);

        private SampleGeneTranscriber() { }

        protected override GeneTreeMutator<SampleGene> GetGeneTreeMutator() =>
            new GeneTreeMutator<SampleGene>(Mutate, DefaultGeneTreeChildrenMutator.Singleton);

        public override SampleGene Sample() =>
            new SampleGene(
                Random.Range(0f, 1f),
                (uint) Random.Range(0, 10),
                (DietaryRestriction) Enum.ToObject(typeof(DietaryRestriction),
                    Enum.GetValues(typeof(DietaryRestriction)).Cast<int>().LastOrDefault()),
                new Limb[] { }
            );

        public override SampleGene Deserialize(JToken gene) => gene.ToObject<SampleGene>();


        public override SampleGene Mutate(SampleGene gene) =>
            new SampleGene(
                FurrinessMutator.Mutate(gene.furriness),
                gene.nEyes.Mutate(.1f),
                gene.dietaryRestriction.Mutate(.1f),
                gene.limbs.Mutate(Limb.Mutator)
            );
    }
}