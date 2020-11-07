using System;
using Cell;
using DefaultNamespace;
using Genetics;
using Newtonsoft.Json.Linq;
using Persistence;
using UnityEngine;

namespace Actuators
{
    [Serializable]
    public class BirthCanalGene
    {
        public float circularMembranePreferredAttachmentAngle = 180f;
        public float circularMembraneAngularDisplacement = 30f;
    }

    public class BirthCanal : MonoBehaviour, ILivingComponent<BirthCanalGene>, IActuator
    {
        private CircularAttachment attachment;
        public BirthCanalGene gene;

        private void Start()
        {
        }

        private void Update()
        {
        }

        public void GiveBirth()
        {
            var geneTree = GeneNode.GetMutated(GetComponentInParent<Cell.Cell>());
            var t = transform;
            var cellColony = GetComponentInParent<CellColony>();
            GeneNode.Load(geneTree, cellColony.transform, t.Find("SpawnPoint").position, t.rotation);
        }

        public float[] Connect() => new float[1];

        public void Actuate(float[] logits)
        {
        }

        public string GetNodeName() => gameObject.name;

        Transform ILivingComponent.OnInheritGene(object inheritedGene) => OnInheritGene((BirthCanalGene) inheritedGene);

        public GeneTranscriber<BirthCanalGene> GetGeneTranscriber() => BirthCanalGeneTranscriber.Singleton;

        public BirthCanalGene GetGene() => gene;

        public Transform OnInheritGene(BirthCanalGene inheritedGene)
        {
            gene = inheritedGene;
            attachment = new CircularAttachment(
                transform,
                inheritedGene.circularMembranePreferredAttachmentAngle,
                inheritedGene.circularMembraneAngularDisplacement
            );
            var membrane = GetComponentInParent<Membrane>();
            membrane.Attach(attachment);
            return transform;
        }

        IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();

        object ILivingComponent.GetGene() => GetGene();

        public string GetResourcePath() => "Organelles/BirthCanal1";

        public JObject GetState() => new JObject();

        public void SetState(JObject state)
        {
        }

        public ILivingComponent[] GetSubLivingComponents() => new ILivingComponent[] { };
    }
}