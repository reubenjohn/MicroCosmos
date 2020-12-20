using System.Collections.Generic;
using Genetics;
using UnityEngine;

namespace Organelles
{
    public static class LivingComponentRegistry
    {
        private static readonly Dictionary<string, ILivingComponent> Registry =
            new Dictionary<string, ILivingComponent>();

        public static ILivingComponent Get(string resource) =>
            Registry.TryGetValue(resource, out var livingComponent)
                ? livingComponent
                : Registry[resource] = Resources.Load<GameObject>(resource).GetComponent<ILivingComponent>();
    }
}