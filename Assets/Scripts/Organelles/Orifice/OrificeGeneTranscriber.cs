using Genetics;
using Newtonsoft.Json.Linq;
using Structural;
using UnityEngine;

namespace Organelles.Orifice
{
    public class OrificeGeneTranscriber : GeneTranscriber<OrificeGene>
    {
        public static readonly OrificeGeneTranscriber Singleton = new OrificeGeneTranscriber();
        private OrificeGeneTranscriber() { }

        public override OrificeGene Sample() =>
            new OrificeGene
            {
                size = Random.Range(.1f, 1f),
                transferRate = Mathf.Pow(10, Random.Range(-2, 0)),
                circularMembraneAttachment = new CircularAttachmentGene
                {
                    preferredAngle = Random.Range(-180f, 180f),
                    angularDisplacement = Random.Range(.1f, 90f)
                }
            };

        public override OrificeGene Deserialize(JToken gene) => gene.ToObject<OrificeGene>();

        public override OrificeGene Mutate(OrificeGene gene) =>
            new OrificeGene
            {
                size = gene.size.MutateClamped(.05f, .1f, 1f),
                transferRate = gene.size.MutateClamped(gene.transferRate * .05f, .1f, 1f),
                circularMembraneAttachment = MutateMembraneAttachment(gene.circularMembraneAttachment)
            };

        private CircularAttachmentGene MutateMembraneAttachment(CircularAttachmentGene attachment) =>
            new CircularAttachmentGene
            {
                preferredAngle = attachment.preferredAngle.Mutate(5f), // TODO Handle overflow
                angularDisplacement = attachment.angularDisplacement.MutateClamped(
                    attachment.angularDisplacement * .1f, .1f, 90f)
            };
    }
}