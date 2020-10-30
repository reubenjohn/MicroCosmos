using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class FlagellaActuator : MonoBehaviour, IActuator, IOrganelle<FlagellaGene>, IOrganelle
{
    public static readonly string IDENTIFFIER = "FlagellaActuator";
    public FlagellaGene gene = new FlagellaGene(250f, 10f);

    private static FlagellaGeneTranscriber GENE_TRANSCRIBER = new FlagellaGeneTranscriber();

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

    public void OnInheritGene(FlagellaGene inheritedGene) => this.gene = inheritedGene;
    void IOrganelle.OnInheritGene(object inheritedGene) => OnInheritGene((FlagellaGene)inheritedGene);

    IGeneTranscriber<FlagellaGene> IOrganelle<FlagellaGene>.GetGeneTranscriber() => GENE_TRANSCRIBER;
    IGeneTranscriber IOrganelle.GetGeneTranscriber() => GENE_TRANSCRIBER;

    public FlagellaGene GetGene() => gene;
    object IOrganelle.GetGene() => GetGene();

    UnityEngine.Object IOrganelle.LoadResource() => Resources.Load("Organelles/Flagella1");

    public Dictionary<string, object> GetState()
    {
        var dict = new Dictionary<string, object>();
        dict.Add("gene", GENE_TRANSCRIBER.Serialize(gene));
        return dict;
    }
    public void SetState(Dictionary<string, object> state)
    {
        throw new NotImplementedException();
    }

}
