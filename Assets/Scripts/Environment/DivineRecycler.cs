using System.Collections;
using Chemistry;
using ChemistryMicro;
using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(ChemicalBlob))]
    public class DivineRecycler : MonoBehaviour
    {
        private static readonly Mixture<Substance> UnitFatMix = new MixtureDictionary<Substance> {{Substance.Fat, 1}}
            .ToMixture();

        private DivineRecycling divineRecycling;

        private ChemicalBlob flask;

        private void Start()
        {
            flask = GetComponent<ChemicalBlob>();
            divineRecycling = DivineRecycling.Instance;
            if (divineRecycling)
                StartCoroutine(StartRecycling());
        }

        private void OnDestroy() => StopAllCoroutines();

        private IEnumerator StartRecycling()
        {
            while (true)
            {
                yield return new WaitForSeconds(divineRecycling.divineRecycleInterval);
                if (flask != null && !Mathf.Approximately(flask[Substance.Fat], flask.TotalMass))
                {
                    Instantiate(Resources.Load<GameObject>("Objects/HaloPop"), transform)
                        .GetComponent<Light>().color = divineRecycling.popAnimationColor;
                    var blobMix = flask.ToMixture();
                    var reaction = new Reaction<Substance>(blobMix, UnitFatMix * blobMix.TotalMass);
                    flask.Convert(reaction);
                }
            }

            // ReSharper disable once IteratorNeverReturns
        }
    }
}