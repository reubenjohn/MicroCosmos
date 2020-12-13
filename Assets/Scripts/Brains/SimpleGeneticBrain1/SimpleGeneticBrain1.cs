using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Brains.SimpleGeneticBrain1
{
    public class SimpleGeneticBrain1 : AbstractBrain, ILivingComponent<SimpleGeneticBrain1Gene>
    {
        private SimpleGeneticBrain1Gene gene;

        private NeuralInterface neuralInterface;

        private new void Start()
        {
            base.Start();
            var livingDescription = new SimpleGeneticBrain1Description(sensorLogits.Length, actuatorLogits.Length);
            if (gene is IRepairableGene<SimpleGeneticBrain1Gene, SimpleGeneticBrain1Description> repairableGene)
                gene = repairableGene.RepairGene(livingDescription);

            neuralInterface = new NeuralInterface(sensorLogits, actuatorLogits, new SimpleNeuralNetwork1(gene));
        }

        public string GetNodeName() => gameObject.name;

        Transform ILivingComponent.OnInheritGene(object inheritedGene) =>
            OnInheritGene((SimpleGeneticBrain1Gene) inheritedGene);

        public GeneTranscriber<SimpleGeneticBrain1Gene> GetGeneTranscriber() =>
            SimpleGeneticBrain1GeneTranscriber.Singleton;

        public SimpleGeneticBrain1Gene GetGene() => gene;

        public Transform OnInheritGene(SimpleGeneticBrain1Gene inheritedGene)
        {
            gene = inheritedGene;
            return transform;
        }

        IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();

        object ILivingComponent.GetGene() => GetGene();

        public string GetResourcePath() => "Organelles/GeneticBrain1";

        public JObject GetState() => new JObject();

        public void SetState(JObject state) { }

        public ILivingComponent[] GetSubLivingComponents() => new ILivingComponent[] { };

        protected override void React() => neuralInterface.React();
    }
}