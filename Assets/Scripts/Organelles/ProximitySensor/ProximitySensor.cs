using System;
using System.Collections.Generic;
using ChemistryMicro;
using Environment;
using Genetics;
using UnityEngine;
using Util;

namespace Organelles.ProximitySensor
{
    public class ProximitySensor : AbstractLivingComponent<ProximitySensorGene>, ISensor
    {
        public static readonly string ResourcePath = "Organelles/ProximitySensor";

        public LayerMask cellLayerMask;
        public LayerMask chemicalBlobLayerMask;
        public LayerMask inertObstacleLayerMask;
        private readonly List<Collider2D> collidersInRange = new List<Collider2D>();
        private Cell.Cell cell;
        private LayerMask proximityLayerMask;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            proximityLayerMask = cellLayerMask | chemicalBlobLayerMask | inertObstacleLayerMask;
        }

        private void Start()
        {
            cell = GetComponentInParent<Cell.Cell>();
            spriteRenderer = transform.Find("Overlay").GetComponent<SpriteRenderer>();
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            var layerFlag = 1 << other.gameObject.layer;
            if ((proximityLayerMask & layerFlag) != 0)
                collidersInRange.Add(other);
        }

        public void OnTriggerExit2D(Collider2D other) => collidersInRange.Remove(other);

        public float[] Connect() => new float[2 + EnumUtils.EnumCount(typeof(Substance))];

        public void Sense(float[] logits)
        {
            for (var i = 0; i < logits.Length; i++)
                logits[i] = -1;
            logits[0] = 1;
            var nUsedLogits = 0;

            collidersInRange.RemoveAll(coll => coll == null);

            spriteRenderer.color = new Color(0, 0, 0, .2f);

            if (collidersInRange.Count > 0)
            {
                var closestColliderArgMin = ArgMin(collidersInRange, DistanceToCollider);
                var closestCollider = closestColliderArgMin.Item1;

                var closestDistance = Mathf.Min(closestColliderArgMin.Item2, 1);
                spriteRenderer.color = Color.Lerp(Color.HSVToRGB(0, 1f, 1f), spriteRenderer.color, closestDistance);
                var distanceLogit = 1 - 2 * closestDistance;
                if (cell.IsInFocus)
                    // TODO Handle multiple proximity sensors
                    Grapher.Log(distanceLogit, "Proximity.Closeness", Color.green);
                logits[nUsedLogits++] = distanceLogit;

                var layerFlag = 1 << closestCollider.gameObject.layer;

                if ((layerFlag & chemicalBlobLayerMask) != 0)
                {
                    var flask = closestCollider.GetComponent<ChemicalBlob>();
                    logits[nUsedLogits++] = -.25f;
                    ActivateChemicalLogits(logits, flask, ref nUsedLogits);
                }

                // else if ((layerFlag & cellContactFilter) != 0)
                // {
                //     flask = closestCollider.GetComponent<CellCauldron.CellCauldron>();
                //     logits[nUsedLogits++] = .25f;
                //     ActivateChemicalLogits(logits, flask, ref nUsedLogits);
                // }
            }
        }

        private void ActivateChemicalLogits(float[] logits, PhysicalFlask flask, ref int nUsedLogits)
        {
            var selfMass = cell.Cauldron.TotalMass;
            foreach (var substance in Enum.GetValues(typeof(Substance)))
            {
                var relativeMass = Mathf.Clamp(flask[(Substance) substance] / selfMass, 0f, float.MaxValue);
                relativeMass = float.IsNaN(relativeMass) ? 0f : relativeMass;
                var logRelativeMass = Mathf.Log10(relativeMass);
                logits[nUsedLogits++] = Mathf.Clamp((logRelativeMass + 1) / 2, 0f, 1f);
            }
        }

        private float DistanceToCollider(Collider2D otherCollider)
        {
            Vector2 pos = transform.position;
            var closestPoint = otherCollider.ClosestPoint(pos);
            var distance = (closestPoint - pos).magnitude;
            return distance;
        }

        private Tuple<T, float> ArgMin<T>(IEnumerable<T> list, Func<T, float> action)
        {
            var min = float.MaxValue;
            T argMin = default;
            foreach (var item in list)
            {
                var val = action.Invoke(item);
                if (val < min)
                {
                    min = val;
                    argMin = item;
                }
            }

            return new Tuple<T, float>(argMin, min);
        }

        public override GeneTranscriber<ProximitySensorGene> GetGeneTranscriber() =>
            ProximitySensorGeneTranscriber.Singleton;

        public override string GetResourcePath() => ResourcePath;
    }
}