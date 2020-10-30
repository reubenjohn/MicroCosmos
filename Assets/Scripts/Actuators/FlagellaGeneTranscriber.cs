using Newtonsoft.Json;

public class FlagellaGeneTranscriber : IGeneTranscriber<FlagellaGene>
{
    public override string Serialize(FlagellaGene gene) => JsonConvert.SerializeObject(gene);

    public override FlagellaGene Deserialize(string sequence) => JsonConvert.DeserializeObject<FlagellaGene>(sequence);

    public override FlagellaGene Mutate(FlagellaGene gene) => new FlagellaGene(
            gene.linearPower.MutateClamped(10f, .1f, float.MaxValue),
            gene.angularPower.MutateClamped(10f, .1f, float.MaxValue)
        );
}
