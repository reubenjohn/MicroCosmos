﻿using Genetics;
using Newtonsoft.Json.Linq;
using TestCommon;
using Tests.Common;
using UnityEngine;

namespace Tests.PlayMode
{
    public class SampleSubLivingComponent : MonoBehaviour, ILivingComponent<SampleSubGene>
    {
        private SampleSubGene gene;
        private JObject state;

        public string GetNodeName() => gameObject.name;

        Transform ILivingComponent.OnInheritGene(object inheritedGene) => OnInheritGene((SampleSubGene) inheritedGene);
        public GeneTranscriber<SampleSubGene> GetGeneTranscriber() => SampleSubGeneTranscriber.Singleton;

        public SampleSubGene GetGene() => gene;

        public Transform OnInheritGene(SampleSubGene inheritedGene)
        {
            gene = inheritedGene;
            return new RectTransform();
        }

        IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();

        object ILivingComponent.GetGene() => GetGene();

        public string GetResourcePath() => "SampleSubOrganelle1";

        public JObject GetState() => state;

        public void SetState(JObject newState) => state = newState;

        public ILivingComponent[] GetSubLivingComponents() => new ILivingComponent[] { };
    }
}