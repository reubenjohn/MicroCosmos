using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Organelles.HunterFlag
{
    public class HunterFlagGeneTranscriber : GeneTranscriber<HunterFlagGene>
    {
        public static readonly HunterFlagGeneTranscriber Singleton = new HunterFlagGeneTranscriber();

        private HunterFlagGeneTranscriber() { }

        public override HunterFlagGene Sample() => new HunterFlagGene(Random.Range(0f, 1f));

        public override HunterFlagGene Deserialize(JToken gene) => gene.ToObject<HunterFlagGene>();

        public override HunterFlagGene Mutate(HunterFlagGene gene) =>
            new HunterFlagGene(
                gene.hue.MutateClamped(gene.hue * .1f, 0f, 1f)
            );
    }
}