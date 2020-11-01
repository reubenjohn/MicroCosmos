using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine;
using System;

public class Membrane : MonoBehaviour, ILivingComponent<MembraneGene>
{
    public static readonly string RESOURCE_PATH = "Organelles/Membrane1";

    void Start() { }

    private void Update() { }

    public MembraneGene GetGene() => new MembraneGene();

    public IGeneTranscriber<MembraneGene> GetGeneTranscriber() => new MembraneGeneTranscriber();

    public string GetNodeName() => gameObject.name;

    public string GetResourcePath() => RESOURCE_PATH;

    public JObject GetState() => new JObject();

    public ILivingComponent[] GetSubLivingComponents() => transform.Children()
        .Select(subTransform => subTransform.GetComponent<ILivingComponent>())
        .Where(e => e != null)
        .ToArray();

    public Transform OnInheritGene(MembraneGene inheritedGene) => transform;

    public Transform OnInheritGene(object inheritedGene) => OnInheritGene((MembraneGene)inheritedGene);

    public void SetState(JObject state) { }

    object ILivingComponent.GetGene() => GetGene();

    IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();
}
