using Newtonsoft.Json.Linq;

public interface IGeneTranscriber
{
    object Deserialize(JToken gene);

    object Mutate(object gene);
}

public abstract class GeneTranscriber<T> : IGeneTranscriber
{
    object IGeneTranscriber.Deserialize(JToken gene) => Deserialize(gene);
    object IGeneTranscriber.Mutate(object gene) => Mutate((T) gene);
    public abstract T Deserialize(JToken gene);

    public abstract T Mutate(T gene);
}