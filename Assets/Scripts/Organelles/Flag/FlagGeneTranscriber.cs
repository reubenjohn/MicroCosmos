using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Organelles.Flag
{
    public class FlagGeneTranscriber : GeneTranscriber<FlagGene>
    {
        public static readonly FlagGeneTranscriber Singleton = new FlagGeneTranscriber();

        private FlagGeneTranscriber() { }

        public override FlagGene Sample() => new FlagGene(Random.Range(0f, 1f));

        public override FlagGene Deserialize(JToken gene) => gene.ToObject<FlagGene>();

        public override FlagGene Mutate(FlagGene gene) =>
            new FlagGene(
                gene.hue.MutateClamped(gene.hue * .1f, 0f, 1f)
            );
    }
}