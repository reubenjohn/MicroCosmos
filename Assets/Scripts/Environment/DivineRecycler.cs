using System;
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

        private static readonly Vector3 Left = Vector3.left * .25f + Vector3.down * .01f;
        private static readonly Vector3 Right = Vector3.right * .25f + Vector3.down * .01f;

        private DivineRecycling divineRecycling;

        private ChemicalBlob flask;
        private float waitStart;

        private void Start()
        {
            flask = GetComponent<ChemicalBlob>();
            divineRecycling = DivineRecycling.Instance;
            if (divineRecycling)
                StartCoroutine(StartRecycling());
        }

        private void OnDestroy() => StopAllCoroutines();

        private void OnDrawGizmosSelected()
        {
            try
            {
                var pos = transform.position;
                var start = pos + Left;
                var end = pos + Right;
                Gizmos.color = Mathf.Approximately(flask[Substance.Fat], flask.TotalMass)
                    ? Color.white
                    : Color.green;
                Gizmos.DrawLine(start, end);
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(start,
                    Vector3.Lerp(start, end, (Time.time - waitStart) / divineRecycling.divineRecycleInterval));
            }
            catch (NullReferenceException) { }
        }

        private IEnumerator StartRecycling()
        {
            while (true)
            {
                waitStart = Time.time;
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