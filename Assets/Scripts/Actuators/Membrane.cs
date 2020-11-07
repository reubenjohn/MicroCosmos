using System;
using System.Linq;
using DefaultNamespace;
using Genetics;
using Newtonsoft.Json.Linq;
using Structural;
using UnityEngine;

namespace Actuators
{
    public class Membrane : MonoBehaviour, ILivingComponent<MembraneGene>
    {
        public static readonly string ResourcePath = "Organelles/Membrane1";
        public CircleCollider2D CircleCollider { get; private set; }
        private CircularAttachmentRing attachmentAdapter;
        public MembraneGene gene; 

        public MembraneGene GetGene() => gene;

        public GeneTranscriber<MembraneGene> GetGeneTranscriber() => MembraneGeneTranscriber.Singleton;

        public string GetNodeName() => gameObject.name;

        public string GetResourcePath() => ResourcePath;

        public JObject GetState() => new JObject();

        public ILivingComponent[] GetSubLivingComponents() => transform.Find("Attachments")
            .Children()
            .Select(subTransform => subTransform.GetComponent<ILivingComponent>())
            .Where(e => e != null)
            .ToArray();

        public Transform OnInheritGene(MembraneGene inheritedGene)
        {
            CircleCollider = GetComponent<CircleCollider2D>();
            CircleCollider.radius = inheritedGene.radius;
            attachmentAdapter = new CircularAttachmentRing(inheritedGene.radius);
            return transform.Find("Attachments");
        }

        public Transform OnInheritGene(object inheritedGene) => OnInheritGene((MembraneGene) inheritedGene);

        public void SetState(JObject state)
        {
        }

        public void Attach(CircularAttachment attachment) => attachmentAdapter.AttachAt(attachment);

        object ILivingComponent.GetGene() => GetGene();

        IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();
    }
}