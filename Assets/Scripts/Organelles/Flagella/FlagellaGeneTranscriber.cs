using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Organelles.Flagella
{
    public class FlagellaGeneTranscriber : GeneTranscriber<FlagellaGene>
    {
        public static readonly FlagellaGeneTranscriber Singleton = new FlagellaGeneTranscriber();

        private FlagellaGeneTranscriber() { }

        public override FlagellaGene Sample() =>
            new FlagellaGene(
                Mathf.Pow(10, Random.Range(1f, 3f)),
                Mathf.Pow(10, Random.Range(2f, 4f))
            );

        public override FlagellaGene Deserialize(JToken gene) => gene.ToObject<FlagellaGene>();

        public override FlagellaGene Mutate(FlagellaGene gene) =>
            new FlagellaGene(
                gene.linearPower.MutateClamped(gene.linearPower * .1f, .1f, float.MaxValue),
                gene.angularPower.MutateClamped(gene.angularPower * .1f, .1f, float.MaxValue)
            );
    }
}