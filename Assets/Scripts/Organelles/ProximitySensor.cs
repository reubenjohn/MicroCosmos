using System;
using System.Collections.Generic;
using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Organelles
{
    public class ProximitySensor : MonoBehaviour, ILivingComponent<ProximitySensorGene>, ISensor
    {
        public ProximitySensorGene gene;
        public ContactFilter2D contactFilter2D;

        private SpriteRenderer spriteRenderer;
        private readonly List<Collider2D> cellCollidersInRange = new List<Collider2D>();

        private bool isInherited;

        private void Start()
        {
            spriteRenderer = transform.Find("Overlay").GetComponent<SpriteRenderer>();
            if (!isInherited)
                OnInheritGene(gene);
        }

        public float[] Connect() => new float[10]; // TODO Match number of substances

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

        public void Sense(float[] logits)
        {
            spriteRenderer.color = new Color(0, 0, 0, .2f);
            foreach (var otherCollider in cellCollidersInRange)
            {
                // var cell = otherCollider.GetComponent<Cell.Cell>();
                Vector2 pos = transform.position;
                var closestPoint = otherCollider.ClosestPoint(pos);
                var distance = (closestPoint - pos).magnitude;
                spriteRenderer.color = Color.Lerp(Color.HSVToRGB(0, 1f, 1f), spriteRenderer.color, distance);
                logits[0] = 1 - 2 * distance;
                Grapher.Log(logits[0], "Proximity.Closeness", Color.green);
            }
        }

        public string GetNodeName() => gameObject.name;

        Transform ILivingComponent.OnInheritGene(object inheritedGene) =>
            OnInheritGene((ProximitySensorGene) inheritedGene);

        public GeneTranscriber<ProximitySensorGene> GetGeneTranscriber() => ProximitySensorGeneTranscriber.Singleton;

        public ProximitySensorGene GetGene() => gene;

        public Transform OnInheritGene(ProximitySensorGene inheritedGene)
        {
            gene = inheritedGene;
            isInherited = true;
            return transform;
        }

        IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();

        object ILivingComponent.GetGene() => GetGene();

        public string GetResourcePath() => "Organelles/ProximitySensor";

        public JObject GetState() => new JObject();

        public void SetState(JObject state)
        {
        }

        public ILivingComponent[] GetSubLivingComponents() => new ILivingComponent[] { };
    }

    [Serializable]
    public class ProximitySensorGene
    {
    }
}