﻿using System.Collections.Generic;
using Genetics;
using UnityEngine;

namespace Organelles
{
    public class ProximitySensor : AbstractLivingComponent<ProximitySensorGene>, ISensor
    {
        public ContactFilter2D contactFilter2D;
        private readonly List<Collider2D> cellCollidersInRange = new List<Collider2D>();
        private Cell.Cell cell;

        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            cell = GetComponentInParent<Cell.Cell>();
            spriteRenderer = transform.Find("Overlay").GetComponent<SpriteRenderer>();
            gene = gene ?? new ProximitySensorGene();
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if ((contactFilter2D.layerMask & (1 << (other.gameObject.layer - 1))) != 0 &&
                other.GetComponentInParent<Cell.Cell>() != null)
                cellCollidersInRange.Add(other);
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            cellCollidersInRange.Remove(other);
        }

        public float[] Connect()
        {
            return new float[10]; // TODO Match number of substances
        }

        public void Sense(float[] logits)
        {
            logits[0] = 0;
            spriteRenderer.color = new Color(0, 0, 0, .2f);
            // TODO Select closest one only
            foreach (var otherCollider in cellCollidersInRange)
            {
                // var cell = otherCollider.GetComponent<Cell.Cell>();
                Vector2 pos = transform.position;
                var closestPoint = otherCollider.ClosestPoint(pos);
                var distance = (closestPoint - pos).magnitude;
                spriteRenderer.color = Color.Lerp(Color.HSVToRGB(0, 1f, 1f), spriteRenderer.color, distance);
                logits[0] = 1 - 2 * distance;
                if (cell.IsInFocus)
                    // TODO Handle multiple proximity sensors
                    Grapher.Log(logits[0], "Proximity.Closeness", Color.green);
            }
        }

        public override GeneTranscriber<ProximitySensorGene> GetGeneTranscriber()
        {
            return ProximitySensorGeneTranscriber.Singleton;
        }

        public override string GetResourcePath()
        {
            return "Organelles/ProximitySensor";
        }
    }
}