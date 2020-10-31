using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class FlagellaGeneTranscriber : IGeneTranscriber<FlagellaGene>
{
    public static FlagellaGeneTranscriber SINGLETON = new FlagellaGeneTranscriber();

    private FlagellaGeneTranscriber() { }

    public override FlagellaGene Deserialize(JToken gene) => gene.ToObject<FlagellaGene>();

    public override FlagellaGene Mutate(FlagellaGene gene) => new FlagellaGene(
            gene.linearPower.MutateClamped(10f, .1f, float.MaxValue),
            gene.angularPower.MutateClamped(10f, .1f, float.MaxValue)
        );
}
