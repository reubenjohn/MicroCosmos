using Genetics;
using Newtonsoft.Json.Linq;
using Structural;
using UnityEngine;

namespace Organelles.BirthCanal
{
    public class BirthCanalGeneTranscriber : GeneTranscriber<BirthCanalGene>
    {
        public static readonly BirthCanalGeneTranscriber Singleton = new BirthCanalGeneTranscriber();

        public override BirthCanalGene Sample() =>
            new BirthCanalGene
            {
                circularMembraneAttachment = new CircularAttachmentGene
                {
                    preferredAngle = Random.Range(-180f, 180f),
                    angularDisplacement = Random.Range(.1f, 90f)
                }
            };

        public override BirthCanalGene Deserialize(JToken gene) => gene.ToObject<BirthCanalGene>();

        public override BirthCanalGene Mutate(BirthCanalGene gene) =>
            new BirthCanalGene
            {
                circularMembraneAttachment = MutateMembraneAttachment(gene.circularMembraneAttachment)
            };

        private CircularAttachmentGene MutateMembraneAttachment(CircularAttachmentGene attachment) =>
            new CircularAttachmentGene
            {
                preferredAngle = attachment.preferredAngle.MutateClamped(
                    5f, -180f, 180f), // TODO Handle overflow
                angularDisplacement = attachment.angularDisplacement.MutateClamped(
                    attachment.angularDisplacement * .1f, .1f, 90f)
            };
    }
}