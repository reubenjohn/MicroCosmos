using System.Linq;
using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Actuators
{
    public class Membrane : MonoBehaviour, ILivingComponent<MembraneGene>
    {
        public static readonly string ResourcePath = "Organelles/Membrane1";

        public MembraneGene GetGene() => new MembraneGene();

        public GeneTranscriber<MembraneGene> GetGeneTranscriber() => new MembraneGeneTranscriber();

        public string GetNodeName() => gameObject.name;

        public string GetResourcePath() => ResourcePath;

        public JObject GetState() => new JObject();

        public ILivingComponent[] GetSubLivingComponents() => transform.Children()
            .Select(subTransform => subTransform.GetComponent<ILivingComponent>())
            .Where(e => e != null)
            .ToArray();

        public Transform OnInheritGene(MembraneGene inheritedGene) => transform;

        public Transform OnInheritGene(object inheritedGene) => OnInheritGene((MembraneGene) inheritedGene);

        public void SetState(JObject state)
        {
        }

        object ILivingComponent.GetGene() => GetGene();

        IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();
    }
}