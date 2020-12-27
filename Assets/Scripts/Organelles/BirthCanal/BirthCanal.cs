using Cell;
using Chemistry;
using ChemistryMicro;
using Environment;
using Genealogy.Graph;
using Genetics;
using Newtonsoft.Json.Linq;
using Persistence;
using Structural;
using UnityEngine;
using Util;

namespace Organelles.BirthCanal
{
    public class BirthCanal : AbstractLivingComponent<BirthCanalGene>, IActuator
    {
        public static readonly string ResourcePath = "Organelles/BirthCanal1";
        public static readonly string ActuatorType = typeof(BirthCanal).FullName;
        public Control.BinaryControlVariable birthSignal = new Control.BinaryControlVariable(1);

        private readonly Mixture<Substance> unitFatMix = new MixtureDictionary<Substance>
        {
            {Substance.Fat, 1f}
        }.ToMixture();

        private CircularAttachment attachment;
        private Cell.Cell cell;

        private Vector3 SpawnPoint => transform.Find("SpawnPoint").position;

        private void Start()
        {
            cell = GetComponentInParent<Cell.Cell>();
        }

        public string GetActuatorType() => ActuatorType;

        public float[] Connect() => new float[1];

        public void Actuate(float[] logits)
        {
            var giveBirthSignal = birthSignal.FeedInput(logits[0] > 0, logits[0] < 0, Time.deltaTime);
            if (Mathf.Approximately(giveBirthSignal, 1))
                TryGiveBirth();
            if (cell.IsInFocus)
            {
                Grapher.Log(logits[0], "GiveBirth?", Color.magenta);
                Grapher.Log(giveBirthSignal, "GiveBirth!", Color.red);
            }
        }

        private void TryGiveBirth()
        {
            birthSignal.Value = 0;

            var geneTree = GeneNode.GetMutated(cell);
            var babyGene = (CellGene) geneTree.gene;
            var babyMix = new Mixture<Substance>(
                EnumUtils.ParseNamedDictionary<Substance, float>(babyGene.cauldron.initialCauldron));
            var cauldron = cell.Cauldron;
            var babyMass = babyMix.TotalMass;
            var mamaFat = cauldron[Substance.Fat];
            if (babyMass >= mamaFat)
                DieMaternally();
            else if (babyMass > mamaFat * .5f)
                Miscarriage(cauldron, unitFatMix * (mamaFat * .5f));
            else
                SpawnBaby(geneTree);
        }

        private void SpawnBaby(GeneNode geneTree)
        {
            Debug.Log("A new cell is being born :)");
            var cellColony = GetComponentInParent<CellColony>();
            var genealogyGraphManager = GetComponentInParent<GenealogyGraphManager>();
            var childGenealogyNode = genealogyGraphManager.RegisterAsexualCellBirth(new Node[] {cell.GenealogyNode});
            var cellState = Cell.Cell.GetState(childGenealogyNode.Guid, SpawnPoint, transform.rotation, new JObject());
            cellColony.SpawnCell(geneTree, cellState, cell.Cauldron);
        }

        private void DieMaternally()
        {
            Debug.Log("Cell is dying through child birth :(");
            cell.Die();
        }

        private void Miscarriage(CellCauldron.CellCauldron cauldron, Mixture<Substance> miscarriageMix)
        {
            Debug.Log("Cell is having a miscarriage :(");
            GetComponentInParent<ChemicalSink>().Dump(SpawnPoint, cauldron, miscarriageMix);
        }

        public override GeneTranscriber<BirthCanalGene> GetGeneTranscriber() => BirthCanalGeneTranscriber.Singleton;

        public override BirthCanalGene GetGene() => gene ?? new BirthCanalGene();

        public override Transform OnInheritGene(BirthCanalGene inheritedGene)
        {
            attachment = new CircularAttachment(
                transform,
                inheritedGene.circularMembraneAttachment
            );
            var membrane = GetComponentInParent<Membrane.Membrane>();
            membrane.Attach(attachment);
            return base.OnInheritGene(inheritedGene);
        }

        public override string GetResourcePath() => ResourcePath;
    }
}