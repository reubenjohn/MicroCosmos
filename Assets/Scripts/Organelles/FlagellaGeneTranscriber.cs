using Genetics;
using Newtonsoft.Json.Linq;

namespace Organelles
{
    public class FlagellaGeneTranscriber : GeneTranscriber<FlagellaGene>
    {
        public static readonly FlagellaGeneTranscriber Singleton = new FlagellaGeneTranscriber();

        private FlagellaGeneTranscriber()
        {
        }

        public override FlagellaGene Deserialize(JToken gene) => gene.ToObject<FlagellaGene>();

        public override FlagellaGene Mutate(FlagellaGene gene) => new FlagellaGene(
            gene.linearPower.MutateClamped(gene.linearPower * .1f, .1f, float.MaxValue),
            gene.angularPower.MutateClamped(gene.angularPower * .1f, .1f, float.MaxValue)
        );
    }
}