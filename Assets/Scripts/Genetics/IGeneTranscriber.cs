using Newtonsoft.Json.Linq;

namespace Genetics
{
    public interface IGeneTranscriber
    {
        object Deserialize(JToken gene);

        object Mutate(object gene);
        GeneTreeMutator GetTreeMutator();

        object Sample();
        IGeneTreeSampler GetTreeSampler();
    }

    public abstract class GeneTranscriber<T> : IGeneTranscriber
    {
        protected GeneTreeMutator<T> geneTreeMutator;

        protected GeneTranscriber()
        {
            geneTreeMutator = geneTreeMutator ??
                              new GeneTreeMutator<T>(Mutate, DefaultGeneTreeChildrenMutator.Singleton);
        }

        object IGeneTranscriber.Deserialize(JToken gene) => Deserialize(gene);
        object IGeneTranscriber.Mutate(object gene) => Mutate((T) gene);
        GeneTreeMutator IGeneTranscriber.GetTreeMutator() => GetGeneTreeMutator();
        object IGeneTranscriber.Sample() => Sample();

        public virtual IGeneTreeSampler GetTreeSampler() => DefaultGeneTreeSampler.Singleton; // TODO Make generic
        public abstract T Sample();

        public abstract T Deserialize(JToken gene);
        public abstract T Mutate(T gene);

        protected virtual GeneTreeMutator<T> GetGeneTreeMutator() => geneTreeMutator;
    }
}