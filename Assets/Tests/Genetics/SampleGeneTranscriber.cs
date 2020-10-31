
using Newtonsoft.Json.Linq;

public class SampleGeneTranscriber : IGeneTranscriber<SampleGene>
{
    internal static readonly SampleGeneTranscriber SINGLETON = new SampleGeneTranscriber();
    public static readonly Mutator.ClampedFloat FURRINESS_MUTATOR = new Mutator.ClampedFloat(0.1f, 0f, 1f);

    private SampleGeneTranscriber() { }

    public override SampleGene Deserialize(JToken gene) => gene.ToObject<SampleGene>();

    public override SampleGene Mutate(SampleGene gene) => new SampleGene(
            FURRINESS_MUTATOR.Mutate(gene.furryness),
            gene.nEyes.Mutate(.1f),
            gene.dietaryRestriction.Mutate(.1f),
            gene.limbs.Mutate(Limb.MUTATOR)
        );
}
