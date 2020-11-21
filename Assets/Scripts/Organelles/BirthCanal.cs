using Cell;
using DefaultNamespace;
using Genealogy;
using Genetics;
using Persistence;
using UnityEditor;
using UnityEngine;

namespace Organelles
{
    public class BirthCanal : AbstractLivingComponent<BirthCanalGene>, IActuator
    {
        public Control.BinaryControlVariable birthSignal = new Control.BinaryControlVariable(1);
        private CircularAttachment attachment;

        public float[] Connect() => new float[1];

        public void Actuate(float[] logits)
        {
            var giveBirthSignal = birthSignal.FeedInput(logits[0] > -.5f, logits[0] < .5, Time.deltaTime);
            if (Mathf.Approximately(giveBirthSignal, 1))
                GiveBirth();
            if (GetComponentInParent<Cell.Cell>().gameObject == Selection.activeGameObject)
            {
                Grapher.Log(logits[0], "GiveBirth?", Color.magenta);
                Grapher.Log(giveBirthSignal, "GiveBirth!", Color.red);
            }
        }

        private void GiveBirth()
        {
            birthSignal.Value = 0;

            var parent = GetComponentInParent<Cell.Cell>();
            var geneTree = GeneNode.GetMutated(parent);

            var t = transform;
            var cellColony = GetComponentInParent<CellColony>();
            var child = GeneNode.Load(geneTree, cellColony.transform, t.Find("SpawnPoint").position, t.rotation);

            var childCell = child.GetComponent<Cell.Cell>();
            if (parent.genealogyGraphManager)
                parent.genealogyGraphManager.RegisterAsexualCellBirth(new Node[] {parent.genealogyNode}, childCell);
        }

        public override GeneTranscriber<BirthCanalGene> GetGeneTranscriber() => BirthCanalGeneTranscriber.Singleton;

        public override Transform OnInheritGene(BirthCanalGene inheritedGene)
        {
            attachment = new CircularAttachment(
                transform,
                inheritedGene.circularMembranePreferredAttachmentAngle,
                inheritedGene.circularMembraneAngularDisplacement
            );
            var membrane = GetComponentInParent<Membrane>();
            membrane.Attach(attachment);
            return base.OnInheritGene(inheritedGene);
        }

        public override string GetResourcePath() => "Organelles/BirthCanal1";
    }
}