using System.Collections.Generic;
using Genetics;
using Newtonsoft.Json;
using UnityEngine;

namespace Organelles.SimpleContainment
{
    public class SubOrganelleCounts : Dictionary<string, float>
    {
        private readonly string[] supportedResources;

        [JsonConstructor]
        public SubOrganelleCounts() { }

        public SubOrganelleCounts(string[] supportedSubLivingComponentsResources)
        {
            supportedResources = supportedSubLivingComponentsResources;
            foreach (var resource in supportedResources)
                this[resource] = GetLogit(resource);
        }

        public SubOrganelleCounts Mutate(float mutationRate)
        {
            var subOrganelleCounts = new SubOrganelleCounts(supportedResources);
            foreach (var resource in supportedResources)
                subOrganelleCounts[resource] = GetLogit(resource).MutateClamped(mutationRate, 0f, .99f);
            return subOrganelleCounts;
        }

        private float GetLogit(string resource) => TryGetValue(resource, out var count) ? count : Random.Range(0f, .9f);

        private int ToCount(float unsignedLogit) => (int) (Mathf.Log(1f / (1 - unsignedLogit)) / Mathf.Log(2));

        public float GetCount(string resource) => ToCount(GetLogit(resource));
    }
}