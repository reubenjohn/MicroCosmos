using Persistence;

namespace Genetics
{
    public interface IGeneTreeSampler
    {
        GeneNode Sample(ILivingComponent livingComponent);
    }
}