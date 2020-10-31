using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Cell : MonoBehaviour, ILivingComponent<CellGene>
{
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() { }

    public void GiveBirth()
    {
        GeneNode geneTree = this.GetMutatedGeneTree();
        LivingComponentUtils.LoadGeneTree(geneTree, transform.parent, transform.position - transform.up * .3f, transform.rotation);
    }

    public string GetNodeName() => gameObject.name;

    public Transform OnInheritGene(CellGene inheritedGene)
    {
        var organellesTransform = transform;
        foreach (Transform existingSubTransforms in organellesTransform)
        {
            Destroy(existingSubTransforms.gameObject);
        }
        return organellesTransform;
    }

    public IGeneTranscriber<CellGene> GetGeneTranscriber() => CellGeneTranscriber.SINGLETON;

    Transform ILivingComponent.OnInheritGene(object inheritedGene) => OnInheritGene((CellGene)inheritedGene);

    IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();

    public CellGene GetGene()
    {
        return new CellGene();
    }

    object ILivingComponent.GetGene() => ((ILivingComponent<CellGene>)this).GetGene();

    public string GetResourcePath() => "Cells/Cell1";

    public JObject GetState()
    {
        var state = new JObject();
        state["position"] = Serialization.ToSerializable(transform.position);
        state["rotation"] = transform.rotation.eulerAngles.z;
        return state;
    }

    public void SetState(JObject state)
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
        JToken position = state["position"];
        transform.position = position != null ? Serialization.ToVector2((string)position) : new Vector2();
        JToken rotation = state["rotation"];
        transform.rotation = rotation != null ? Quaternion.Euler(0, 0, (float)rotation) : new Quaternion();
    }

    public ILivingComponent[] GetSubLivingComponents()
    {
        return transform.Children()
            .Select(organelleTransform => organelleTransform.GetComponent<ILivingComponent>())
            .Where(e => e != null)
            .ToArray();
    }
}
