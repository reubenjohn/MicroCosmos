using Persistence;

namespace Genetics
{
    public delegate GeneNode[] GeneTreeChildrenMutator(GeneNode parent);

    public delegate T GeneMutator<T>(T gene);

    public delegate object GeneMutator(object gene);
}