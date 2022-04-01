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
        public const string ResourcePath = "Organelles/ProximitySensor";

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

        public float[] Connect() => new float[3 + SubstanceHelper.NSubstances];

        public void Sense(float[] logits)
        {
            for (var i = 0; i < logits.Length; i++)
                logits[i] = -1;
            logits[0] = -1;
            logits[1] = -1;
            logits[2] = -1;

            collidersInRange.RemoveAll(coll => coll == null);

            spriteRenderer.color = new Color(0, 0, 0, .2f);

            if (collidersInRange.Count > 0)
            {
                var (closestCollider, closestDist) = ArrayUtils.ArgMin(collidersInRange, DistanceToCollider);

                var normalizedDist = Mathf.Min(closestDist, 1);
                spriteRenderer.color = Color.Lerp(Color.HSVToRGB(0, 1f, 1f), spriteRenderer.color, normalizedDist);
                var distanceLogit = 1 - normalizedDist;
                if (cell.IsInFocus)
                    Grapher.Log(distanceLogit, "Proximity.Closeness",
                        Color.green); // TODO Handle multiple proximity sensors


                var layerFlag = 1 << closestCollider.gameObject.layer;

                string targetType = null;
                if ((layerFlag & cellLayerMask) != 0)
                {
                    targetType = "CELL";
                    logits[0] = distanceLogit;
                }
                else if ((layerFlag & chemicalBlobLayerMask) != 0)
                {
                    targetType = "CHEMICAL_BLOB";
                    logits[1] = distanceLogit;
                }
                else if ((layerFlag & inertObstacleLayerMask) != 0)
                {
                    targetType = "INERT";
                    logits[2] = distanceLogit;
                }

                var nUsedLogits = 3;

                if (targetType == "CHEMICAL_BLOB")
                {
                    var flask = closestCollider.GetComponent<ChemicalBlob>();
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
            foreach (var substance in SubstanceHelper.Substances)
            {
                var relativeMass = Mathf.Clamp(flask[substance] / selfMass, 0f, float.MaxValue);
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

        public override GeneTranscriber<ProximitySensorGene> GetGeneTranscriber() =>
            ProximitySensorGeneTranscriber.Singleton;

        public override string GetResourcePath() => ResourcePath;
    }
}