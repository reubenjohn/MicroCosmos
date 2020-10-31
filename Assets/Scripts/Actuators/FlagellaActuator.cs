﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class FlagellaActuator : MonoBehaviour, IActuator, ILivingComponent<FlagellaGene>, ILivingComponent
{
    public static readonly string IDENTIFFIER = "FlagellaActuator";
    public static readonly string RESOURCE_PATH = "Organelles/Flagella1";

    public FlagellaGene gene = new FlagellaGene(250f, 10f);
    public Rigidbody2D rb { get; private set; }

    void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    public float[] Connect()
    {
        return new float[2];
    }

    public void Actuate(float[] logits)
    {
        Grapher.Log(logits[0], "Flagella[0]", Color.blue);
        Grapher.Log(logits[1], "Flagella[1]", Color.cyan);
        rb.AddRelativeForce(logits[0] * gene.linearPower * Time.deltaTime * Vector2.up);
        rb.AddTorque(logits[1] * gene.angularPower * Time.deltaTime);
    }

    public string GetNodeName() => gameObject.name;

    public Transform OnInheritGene(FlagellaGene inheritedGene)
    {
        this.gene = inheritedGene;
        return null;
    }
    Transform ILivingComponent.OnInheritGene(object inheritedGene) => OnInheritGene((FlagellaGene)inheritedGene);

    public IGeneTranscriber<FlagellaGene> GetGeneTranscriber() => FlagellaGeneTranscriber.SINGLETON;
    IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();

    public FlagellaGene GetGene() => gene;
    object ILivingComponent.GetGene() => GetGene();

    string ILivingComponent.GetResourcePath() => RESOURCE_PATH;

    public JObject GetState()
    {
        var dict = new JObject();
        // dict.Add("gene", GENE_TRANSCRIBER.Serialize(gene));
        return dict;
    }
    public void SetState(JObject state) { }

    public ILivingComponent[] GetSubLivingComponents() => new ILivingComponent[] { };
}
