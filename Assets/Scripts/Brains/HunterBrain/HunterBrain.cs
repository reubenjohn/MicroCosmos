using System;
using System.Linq;
using Environment;
using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Brains.HunterBrain
{
    public class HunterBrain : AbstractBrain, ILivingComponent<HunterBrainGene>
    {
        public const string ResourcePath = "Organelles/HunterBrain";
        private HunterBrainGene gene;
        private Cell.Cell cell;
        private readonly Guid hunterAGuid = Guid.Parse(SimParams.Singleton.hunterBaseAGuid);
        private readonly Guid hunterBGuid = Guid.Parse(SimParams.Singleton.hunterBaseBGuid);

        protected override void Start()
        {
            cell = GetComponentInParent<Cell.Cell>();
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
            try
            {
                if (IsHomeBase()) ReactLikeHomeBase();
                else ReactLikeHunter();
            }
            catch (InputSizeMismatchException)
            {
                ResetBrain();
                React();
            }
        }

        private void ReactLikeHomeBase()
        {
            actuatorLogits[SimParams.Singleton.birthCanalIndex][0] = 1f; // Birth
        }

        private void ReactLikeHunter()
        {
            actuatorLogits[SimParams.Singleton.flagellaIndex][0] = 1f; // Force
            actuatorLogits[SimParams.Singleton.flagellaIndex][1] = -0.03f; // Torque
        }

        private bool IsHomeBase() => cell.GenealogyNode.Guid == hunterAGuid || cell.GenealogyNode.Guid == hunterBGuid;
    }
}