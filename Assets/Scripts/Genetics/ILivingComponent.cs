using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public interface ILivingComponent
{
    string GetNodeName();
    Transform OnInheritGene(object inheritedGene);
    IGeneTranscriber GetGeneTranscriber();
    object GetGene();
    string GetResourcePath();

    JObject GetState();
    void SetState(JObject state);

    ILivingComponent[] GetSubLivingComponents();
}


public interface ILivingComponent<T> : ILivingComponent
{
    Transform OnInheritGene(T inheritedGene);
    new IGeneTranscriber<T> GetGeneTranscriber();
    new T GetGene();
}

public static class LivingComponentUtils
{
    public static GeneNode SaveGeneTree(this ILivingComponent livingComponent)
    {
        return new GeneNode()
        {
            resource = livingComponent.GetResourcePath(),
            name = livingComponent.GetNodeName(),
            gene = livingComponent.GetGene(),
            children = livingComponent.GetSubLivingComponents()
                .Select(subLivingComponent => subLivingComponent.SaveGeneTree())
                .ToArray()
        };
    }

    public static GameObject LoadGeneTree(GeneNode geneNode, Transform container)
    {
        GameObject gameObject = (GameObject)GameObject.Instantiate(Resources.Load(geneNode.resource), container);
        ILivingComponent livingComponent = gameObject.GetComponent<ILivingComponent>();
        gameObject.name = geneNode.name;
        Transform subLivingComponentContainer = livingComponent.OnInheritGene(geneNode.gene);
        foreach (var subGeneNode in geneNode.children)
        {
            LoadGeneTree(subGeneNode, subLivingComponentContainer);
        }
        return gameObject;
    }

    public static StateNode SaveStateTree(this ILivingComponent livingComponent)
    {
        return new StateNode()
        {
            state = livingComponent.GetState(),
            children = livingComponent.GetSubLivingComponents()
                .Select(subLivingComponent => subLivingComponent.SaveStateTree())
                .ToArray()
        };
    }
    public static void LoadStateTree(this ILivingComponent livingComponent, StateNode stateNode)
    {
        livingComponent.SetState(stateNode.state);
        Enumerable.Zip(livingComponent.GetSubLivingComponents(), stateNode.children, (subLivingComponent, subStateNode) =>
        {
            subLivingComponent.LoadStateTree(subStateNode);
            return true;
        });
    }
}

public class StateNode
{
    public JObject state;
    public StateNode[] children;
}

[JsonConverter(typeof(GeneNodeJsonConverter))]
public class GeneNode
{
    public string resource;
    public string name;
    public object gene;
    public GeneNode[] children;

    [JsonIgnore]
    public ILivingComponent livingComponent;
}

public class GeneNodeJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(GeneNode).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        JObject jsonObject = JObject.Load(reader);

        var geneNode = (existingValue as GeneNode ?? new GeneNode());

        JToken resource = jsonObject["resource"];
        geneNode.resource = (string)resource;
        resource.Parent.Remove();

        geneNode.livingComponent = ((GameObject)Resources.Load(geneNode.resource)).GetComponent<ILivingComponent>();

        JToken gene = jsonObject["gene"];
        geneNode.gene = geneNode.livingComponent.GetGeneTranscriber().Deserialize(gene);
        gene.Parent.Remove();


        using (var subReader = jsonObject.CreateReader())
            serializer.Populate(subReader, geneNode);

        return geneNode;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanWrite { get => false; }
}