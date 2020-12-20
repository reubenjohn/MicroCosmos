using Genetics;
using Newtonsoft.Json.Linq;

namespace Organelles.SimpleContainment
{
    public abstract class AbstractContainmentGeneTranscriber<T> : GeneTranscriber<T>
    {
        protected AbstractContainmentGeneTranscriber()
        {
            geneTreeMutator = new GeneTreeMutator<T>(Mutate, ContainedOrganelleMutator.GetMutatedChildren);
        }

        public abstract override T Sample();

        public override IGeneTreeSampler GetTreeSampler() => ContainingGeneTreeSampler.Singleton;

        public override T Deserialize(JToken gene) => gene.ToObject<T>();

        public abstract override T Mutate(T gene);
    }
}