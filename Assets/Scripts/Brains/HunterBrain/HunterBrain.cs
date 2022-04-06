using System;
using System.Linq;
using Environment;
using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Util;

namespace Brains.HunterBrain
{
    public class HunterBrain : AbstractBrain, ILivingComponent<HunterBrainGene>
    {
        public const string ResourcePath = "Organelles/HunterBrain";

        private LayerMask proximityLayerMask;

        private GenealogyGraphManager graphManager;
        private HunterBrainGene gene;
        private Cell.Cell cell;
        private Vector2 cellPos;
        private Collider2D[] collidersInRange;

        protected override void Start()
        {
            proximityLayerMask = SimParams.Singleton.cellLayerMask |
                                 SimParams.Singleton.inertObstacleLayerMask;
            collidersInRange = new Collider2D[20];

            cell = GetComponentInParent<Cell.Cell>();
            graphManager = GetComponentInParent<GenealogyGraphManager>();
            base.Start();
            ResetBrain();
        }

        public string GetNodeName() => gameObject.name;

        Transform ILivingComponent.OnInheritGene(object inheritedGene) => OnInheritGene((HunterBrainGene)inheritedGene);

        public GeneTranscriber<HunterBrainGene> GetGeneTranscriber() => HunterBrainGeneTranscriber.Singleton;

        public HunterBrainGene GetGene() => gene;

        public Transform OnInheritGene(HunterBrainGene inheritedGene)
        {
            gene = inheritedGene;
            return transform;
        }

        IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();

        object ILivingComponent.GetGene() => GetGene();

        public string GetResourcePath() => ResourcePath;

        public JObject GetState() => new JObject();

        public void SetState(JObject state) { }

        public ILivingComponent[] GetSubLivingComponents() => new ILivingComponent[] { };

        private void ResetBrain()
        {
            var livingDescription = new HunterBrainDescription(
                sensorLogits.Select(logits => logits.Length).Sum(),
                actuatorLogits.Select(logits => logits.Length).Sum()
            );
            if (gene is IRepairableGene<HunterBrainGene, HunterBrainDescription> repairableGene)
                gene = repairableGene.RepairGene(livingDescription);
        }

        protected override void React()
        {
            if (MicroHunters.IsHomeBase(cell)) ReactLikeHomeBase();
            else ReactLikeHunter();
        }

        private void ReactLikeHomeBase()
        {
            actuatorLogits[SimParams.Singleton.birthCanalIndex][0] = 1f; // Birth
        }


        private void ReactLikeHunter()
        {
            // environment.CellCount; // Count of cells in the environment
            var cellTransform = cell.transform;
            cellPos = cellTransform.position;
            var nDetected = Physics2D.OverlapCircleNonAlloc(cellPos,
                cellTransform.localScale.magnitude * SimParams.Singleton.hunterVisibilityRangeRatio, collidersInRange,
                proximityLayerMask.value);

            if (nDetected <= 0)
                return;

            var (closestCollider, dist) = ArrayUtils.ArgMin(collidersInRange.Take(nDetected), DistanceToCollider);
            if (closestCollider == null)
                return;

            var layerFlag = 1 << closestCollider.gameObject.layer;

            if ((layerFlag & SimParams.Singleton.cellLayerMask) == 0)
                return;

            var otherCell = closestCollider.GetComponentInParent<Cell.Cell>();
            var classification = MicroHunters.ClassifyCell(otherCell);

            // CellType type = MicroHunters.GetCellType(classification);
            // HunterTeam team = MicroHunters.GetHunterTeam(classification);
            // int TeamACount = MicroHunters.TeamAHunters.Count();
            // int TeamBCount = MicroHunters.TeamAHunters.Count();

            if (classification == CellClassification.BaseA)
            {
                // Debug.Log("Home base A found");
            }
            else if (classification == CellClassification.BaseB)
            {
                // Debug.Log("Home base B found");
            }
            else if (classification == CellClassification.HunterA)
            {
                // Debug.Log("Hunter A found");
            }
            else if (classification == CellClassification.HunterB)
            {
                // Debug.Log("Hunter B found");
            }
            else if (classification == CellClassification.Sheep)
            {
                // Debug.Log("Sheep found");
            }

            actuatorLogits[SimParams.Singleton.flagellaIndex][0] = 1f; // Force
            actuatorLogits[SimParams.Singleton.flagellaIndex][1] = -0.03f; // Torque
            actuatorLogits[SimParams.Singleton.orificeIndex][0] = 1f; // Eat
        }


        private float DistanceToCollider(Collider2D otherCollider)
        {
            var closestPoint = otherCollider.ClosestPoint(cellPos);
            var distance = (closestPoint - cellPos).magnitude;
            return distance == 0 ? float.MaxValue : distance;
        }
    }
}