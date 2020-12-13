using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Organelles.BirthCanal
{
    public class BirthCanalGeneTranscriber : GeneTranscriber<BirthCanalGene>
    {
        public static readonly BirthCanalGeneTranscriber Singleton = new BirthCanalGeneTranscriber();

        public override BirthCanalGene Sample() =>
            new BirthCanalGene
            {
                circularMembranePreferredAttachmentAngle = Random.Range(-180f, 180f), // TODO Handle overflow
                circularMembraneAngularDisplacement = Random.Range(.1f, 90f)
            };

        public override BirthCanalGene Deserialize(JToken gene) => gene.ToObject<BirthCanalGene>();

        public override BirthCanalGene Mutate(BirthCanalGene gene) =>
            new BirthCanalGene
            {
                circularMembranePreferredAttachmentAngle = gene.circularMembranePreferredAttachmentAngle.MutateClamped(
                    5f, -180f, 180f), // TODO Handle overflow
                circularMembraneAngularDisplacement =
                    gene.circularMembraneAngularDisplacement.MutateClamped(
                        gene.circularMembraneAngularDisplacement * .1f, .1f, 90f)
            };
    }
}