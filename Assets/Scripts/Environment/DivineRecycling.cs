using System.Collections.Generic;
using Chemistry;
using ChemistryMicro;
using UnityEngine;

namespace Environment
{
    public class DivineRecycling : MonoBehaviour
    {
        public float divineRecycleInterval = 1;

        private readonly Dictionary<int, float> creationTimes = new Dictionary<int, float>();

        private void Update()
        {
            if (Time.frameCount % 11 == 0 && divineRecycleInterval > 0)
                foreach (var blob in transform.GetComponentsInChildren<ChemicalBlob>())
                {
                    var instanceID = blob.GetInstanceID();
                    if (creationTimes.TryGetValue(instanceID, out var creationTime))
                    {
                        if (Time.time - creationTime > divineRecycleInterval)
                        {
                            if (Mathf.Approximately(blob[Substance.Fat], blob.TotalMass))
                                break;
                            var blobMix = blob.ToMixture();
                            var fatMix = new MixtureDictionary<Substance> {{Substance.Fat, blobMix.TotalMass}}
                                .ToMixture();
                            var reaction = new Reaction<Substance>(blobMix, fatMix);
                            blob.Convert(reaction);
                        }
                    }
                    else
                    {
                        creationTimes[instanceID] = Time.time;
                    }
                }
        }
    }
}