using System.Collections.Generic;
using Genetics;
using Newtonsoft.Json.Linq;
using Tests.Genetics;
using UnityEngine;

namespace PlayModeTests
{
    public class SampleLivingComponent : MonoBehaviour, ILivingComponent<SampleGene>
    {
        private const string ResourcePath = "SampleOrganelle1";

        private SampleGene gene;
        private JObject state;

        public SampleLivingComponent() => state = new JObject {["x"] = 1, ["y"] = 2};

        public string GetNodeName() => gameObject.name;

        Transform ILivingComponent.OnInheritGene(object inheritedGene) => OnInheritGene((SampleGene) inheritedGene);

        public GeneTranscriber<SampleGene> GetGeneTranscriber() => SampleGeneTranscriber.Singleton;

        public SampleGene GetGene() => gene;

        public Transform OnInheritGene(SampleGene inheritedGene)
        {
            gene = inheritedGene;
            return transform;
        }

        IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();

        object ILivingComponent.GetGene() => GetGene();

        public string GetResourcePath() => ResourcePath;

        public JObject GetState() => state;

        public void SetState(JObject newState) => state = newState;

        public ILivingComponent[] GetSubLivingComponents()
        {
            var list = new List<ILivingComponent>();
            foreach (Transform child in transform)
                if (child.TryGetComponent(out SampleSubLivingComponent sub))
                    list.Add(sub);
            return list.ToArray();
        }
    }
}