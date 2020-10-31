using Newtonsoft.Json.Linq;

public class MembraneGeneTranscriber : IGeneTranscriber<MembraneGene>
{
    public override MembraneGene Deserialize(JToken gene) => gene.ToObject<MembraneGene>();

    public override MembraneGene Mutate(MembraneGene gene) => new MembraneGene();
}