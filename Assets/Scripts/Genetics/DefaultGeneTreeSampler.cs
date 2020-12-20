using Persistence;

namespace Genetics
{
    public class DefaultGeneTreeSampler : IGeneTreeSampler
    {
        public static readonly DefaultGeneTreeSampler Singleton = new DefaultGeneTreeSampler();

        protected DefaultGeneTreeSampler() { }

        public virtual GeneNode Sample(ILivingComponent livingComponent) =>
            new GeneNode(livingComponent, livingComponent.GetGeneTranscriber().Sample(), new GeneNode[0]);
    }
}