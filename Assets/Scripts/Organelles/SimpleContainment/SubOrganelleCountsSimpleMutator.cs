using System.Collections.Generic;
using System.Linq;
using Genetics;

namespace Organelles.SimpleContainment
{
    public class SubOrganelleCountsSimpleMutator
    {
        private readonly Dictionary<string, GeneMutator<float>> countMutators;

        public SubOrganelleCountsSimpleMutator(Dictionary<string, GeneMutator<float>> countMutators)
        {
            this.countMutators = countMutators;
        }

        public SubOrganelleCounts Mutate(SubOrganelleCounts counts)
        {
            var subOrganelleCounts = new SubOrganelleCounts(countMutators.Keys.ToArray());
            foreach (var pair in countMutators)
                subOrganelleCounts[pair.Key] = pair.Value(counts.GetLogit(pair.Key));
            return subOrganelleCounts;
        }
    }
}