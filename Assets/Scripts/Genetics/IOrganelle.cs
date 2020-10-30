using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOrganelle
{
    void OnInheritGene(object inheritedGene);
    IGeneTranscriber GetGeneTranscriber();
    object GetGene();
    UnityEngine.Object LoadResource();

    Dictionary<string, object> GetState();
    void SetState(Dictionary<string, object> state);
}


public interface IOrganelle<T> : IOrganelle
{
    void OnInheritGene(T inheritedGene);
    new IGeneTranscriber<T> GetGeneTranscriber();
    new T GetGene();
}
