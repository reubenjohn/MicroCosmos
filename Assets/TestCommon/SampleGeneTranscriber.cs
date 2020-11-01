using Genetics;
using Newtonsoft.Json.Linq;

namespace Tests.Genetics
{
    public class SampleGeneTranscriber : GeneTranscriber<SampleGene>
    {
        public static readonly SampleGeneTranscriber Singleton = new SampleGeneTranscriber();
        public static readonly Mutator.ClampedFloat FurrinessMutator = new Mutator.ClampedFloat(0.1f, 0f, 1f);

        private SampleGeneTranscriber() { }

        public override SampleGene Deserialize(JToken gene) => gene.ToObject<SampleGene>();

        public override SampleGene Mutate(SampleGene gene) => new SampleGene(
            FurrinessMutator.Mutate(gene.furriness),
            gene.nEyes.Mutate(.1f),
            gene.dietaryRestriction.Mutate(.1f),
            gene.limbs.Mutate(Limb.Mutator)
        );
    }
}
