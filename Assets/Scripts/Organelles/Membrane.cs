using System.Linq;
using DefaultNamespace;
using Genetics;
using Structural;
using UnityEngine;

namespace Organelles
{
    public class Membrane : AbstractLivingComponent<MembraneGene>
    {
        public static readonly string ResourcePath = "Organelles/Membrane1";
        private readonly CircularAttachmentRing attachmentAdapter = new CircularAttachmentRing();
        public CircleCollider2D CircleCollider { get; private set; }

        public override GeneTranscriber<MembraneGene> GetGeneTranscriber() => MembraneGeneTranscriber.Singleton;

        public override string GetResourcePath() => ResourcePath;

        public override ILivingComponent[] GetSubLivingComponents() => transform.Find("Attachments")
            .Children()
            .Select(subTransform => subTransform.GetComponent<ILivingComponent>())
            .Where(e => e != null)
            .ToArray();

        public override MembraneGene GetGene() => gene ?? new MembraneGene();

        public override Transform OnInheritGene(MembraneGene inheritedGene)
        {
            base.OnInheritGene(inheritedGene);
            CircleCollider = GetComponent<CircleCollider2D>();
            transform.localScale = Vector3.one * inheritedGene.radius;
            return transform.Find("Attachments");
        }

        public void Attach(CircularAttachment attachment) => attachmentAdapter.AttachAt(attachment);
    }
}