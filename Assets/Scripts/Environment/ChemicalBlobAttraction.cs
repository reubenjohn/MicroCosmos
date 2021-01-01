using System;
using System.Collections;
using ChemistryMicro;
using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(PhysicalFlask), typeof(Rigidbody2D))]
    public class ChemicalBlobAttraction : MonoBehaviour
    {
        private readonly AttractionTarget target = new AttractionTarget();

        private ChemicalBlob blob;

        private MicroCosmosParameters microCosmosParameters;
        private Rigidbody2D rb;

        private void Start()
        {
            blob = GetComponent<ChemicalBlob>();
            rb = GetComponent<Rigidbody2D>();
            microCosmosParameters = MicroCosmosParameters.Instance;
            if (microCosmosParameters)
                StartCoroutine(CoalesceSearch());
        }

        private void Update()
        {
            if (target.IsTargetSet)
            {
                if (blob.TotalMass > ChemicalBlob.MinBlobSize)
                {
                    var displacement = target.Transform.position - transform.position;
                    var scaleFactor = microCosmosParameters.coalesceChemicalBlobs.strength * target.AttractionFactor *
                                      Mathf.Sqrt(blob.TotalMass) *
                                      displacement.magnitude;
                    rb.AddForce(displacement * scaleFactor, ForceMode2D.Force);
                }
                else
                {
                    blob.CoalesceInto(target.Blob);
                }
            }
        }

        private void OnDestroy() => StopAllCoroutines();

        private void OnDrawGizmosSelected()
        {
            if (microCosmosParameters != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, microCosmosParameters.coalesceChemicalBlobs.cutOffRange);
                if (target.IsTargetSet)
                {
                    Gizmos.color = Color.green;
                    var targetPos = target.Transform.position;
                    Gizmos.DrawRay(targetPos, transform.position - targetPos);
                }
            }
        }

        private IEnumerator CoalesceSearch()
        {
            while (true)
            {
                yield return new WaitForSeconds(microCosmosParameters.coalesceChemicalBlobs.attractionInterval);
                if (target.IsTargetSet)
                {
                    target.StrengthenAttraction();
                }
                else
                {
                    var pos = transform.position;
                    var other = Physics2D.OverlapCircle(pos, microCosmosParameters.coalesceChemicalBlobs.cutOffRange,
                        microCosmosParameters.coalesceChemicalBlobs.layerMask);

                    if (other != null && other.TryGetComponent<ChemicalBlob>(out var otherBlob) &&
                        otherBlob.TotalMass >= blob.TotalMass)
                        target.SetTarget(otherBlob);
                    else
                        target.ClearTarget();
                }
            }

            // ReSharper disable once IteratorNeverReturns
        }

        [Serializable]
        public class GlobalConfiguration
        {
            public float attractionInterval = 1f;
            public float cutOffRange = 1f;
            public LayerMask layerMask;
            public float strength = 1f;
        }
    }

    public class AttractionTarget
    {
        public Transform Transform { get; private set; }
        public ChemicalBlob Blob { get; private set; }
        public float AttractionFactor { get; private set; }

        public bool IsTargetSet
        {
            get
            {
                if (AttractionFactor > 0)
                {
                    if (Blob != null)
                    {
                        return true;
                    }

                    ClearTarget();
                    return false;
                }

                return false;
            }
        }

        public void SetTarget(ChemicalBlob blob)
        {
            Blob = blob;
            Transform = blob.transform;
            AttractionFactor = 1f;
        }

        public void StrengthenAttraction() => AttractionFactor = Mathf.Min(1e12f, AttractionFactor * 2f);

        public void ClearTarget()
        {
            Blob = null;
            Transform = null;
            AttractionFactor = 0;
        }
    }
}