using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Tests
{
    public class SampleLivingComponent : ILivingComponent<SampleGene>
    {
        private JObject state;

        private readonly ILivingComponent[] subLivingComponents =
            {new SampleSubLivingComponent(), new SampleSubLivingComponent(),};

        public SampleLivingComponent() => state = new JObject() {["x"] = 1, ["y"] = 2};

        public string GetNodeName() => "MySampleLivingComponent";

        public Transform OnInheritGene(object inheritedGene) => new RectTransform
            {name = "childTransform", position = new Vector3(1, 2)};

        public IGeneTranscriber<SampleGene> GetGeneTranscriber() => throw new System.NotImplementedException();

        public SampleGene GetGene() => throw new System.NotImplementedException();

        public Transform OnInheritGene(SampleGene inheritedGene) => throw new System.NotImplementedException();

        IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();

        object ILivingComponent.GetGene() => GetGene();

        public string GetResourcePath() => throw new System.NotImplementedException();

        public JObject GetState() => state;

        public void SetState(JObject newState) => state = newState;

        public ILivingComponent[] GetSubLivingComponents() => subLivingComponents;
    }

    public class SampleSubLivingComponent : ILivingComponent
    {
        private JObject state;

        public string GetNodeName() => throw new System.NotImplementedException();

        public Transform OnInheritGene(object inheritedGene) => throw new System.NotImplementedException();

        public IGeneTranscriber GetGeneTranscriber() => throw new System.NotImplementedException();

        public object GetGene() => throw new System.NotImplementedException();

        public string GetResourcePath() => throw new System.NotImplementedException();

        public JObject GetState() => state;

        public void SetState(JObject newState) => state = newState;

        public ILivingComponent[] GetSubLivingComponents() => new ILivingComponent[] { };
    }
}