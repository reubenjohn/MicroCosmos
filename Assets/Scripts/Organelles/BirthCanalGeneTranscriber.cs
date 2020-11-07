using Genetics;
using Newtonsoft.Json.Linq;

namespace Organelles
{
    public class BirthCanalGeneTranscriber : GeneTranscriber<BirthCanalGene>
    {
        public static readonly BirthCanalGeneTranscriber Singleton = new BirthCanalGeneTranscriber();

        public override BirthCanalGene Deserialize(JToken gene) => gene.ToObject<BirthCanalGene>();

        public override BirthCanalGene Mutate(BirthCanalGene gene)
        {
            return new BirthCanalGene()
            {
                circularMembranePreferredAttachmentAngle = gene.circularMembranePreferredAttachmentAngle.MutateClamped(
                    5f, -180f, 180f), // TODO Handle overflow
                circularMembraneAngularDisplacement =
                    gene.circularMembraneAngularDisplacement.MutateClamped(
                        gene.circularMembraneAngularDisplacement * .1f, .1f, 90f)
            };
        }
    }
}