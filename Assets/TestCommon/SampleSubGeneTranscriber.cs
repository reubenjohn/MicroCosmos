using Genetics;
using Newtonsoft.Json.Linq;
using TestCommon;

namespace Tests.Genetics
{
    public class SampleSubGeneTranscriber : GeneTranscriber<SampleSubGene>
    {
        public static readonly SampleSubGeneTranscriber Singleton = new SampleSubGeneTranscriber();
        public static readonly Mutator.ClampedFloat HappinessMutator = new Mutator.ClampedFloat(0.1f, 0f, 1f);

        private SampleSubGeneTranscriber() { }

        public override SampleSubGene Deserialize(JToken gene) => gene.ToObject<SampleSubGene>();

        public override SampleSubGene Mutate(SampleSubGene gene) => new SampleSubGene(
            HappinessMutator.Mutate(gene.happiness)
        );
    }
}
