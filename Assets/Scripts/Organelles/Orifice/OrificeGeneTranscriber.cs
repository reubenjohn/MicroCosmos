using Genetics;
using Newtonsoft.Json.Linq;
using Structural;
using UnityEngine;

namespace Organelles.Orifice
{
    public class OrificeGeneTranscriber : GeneTranscriber<OrificeGene>
    {
        public static readonly OrificeGeneTranscriber Singleton = new OrificeGeneTranscriber();

        private static readonly float MinTransferRateLog10 = -3;
        private static readonly float MinTransferRate = Mathf.Pow(10, MinTransferRateLog10);
        private static readonly float MaxTransferRateLog10 = -1;
        private static readonly float MaxTransferRate = Mathf.Pow(10, MaxTransferRateLog10);
        private OrificeGeneTranscriber() { }

        public override OrificeGene Sample() =>
            new OrificeGene
            {
                size = Random.Range(.1f, 1f),
                transferRate = Mathf.Pow(10, Random.Range(MinTransferRateLog10, MaxTransferRateLog10)),
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
                transferRate =
                    gene.transferRate.MutateClamped(gene.transferRate * .05f, MinTransferRate, MaxTransferRate),
                circularMembraneAttachment = MutateMembraneAttachment(gene.circularMembraneAttachment)
            };

        private CircularAttachmentGene MutateMembraneAttachment(CircularAttachmentGene attachment) =>
            new CircularAttachmentGene
            {
                preferredAngle = attachment.preferredAngle.Mutate(5f), // TODO Handle overflow
                angularDisplacement = attachment.angularDisplacement.MutateClamped(
                    attachment.angularDisplacement * .1f, .1f, 90f)
            };

        public static float NormalizedTransferRate(float transferRate) =>
            NormalizedTransferRate(MinTransferRateLog10, MaxTransferRateLog10, transferRate);

        public static float NormalizedTransferRate(float minTransferRateLog10, float maxTransferRateLog10,
            float transferRate) =>
            (Mathf.Log10(transferRate) - minTransferRateLog10) / (maxTransferRateLog10 - minTransferRateLog10);
    }
}