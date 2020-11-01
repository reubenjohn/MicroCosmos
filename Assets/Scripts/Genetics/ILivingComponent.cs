using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Genetics
{
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
        new GeneTranscriber<T> GetGeneTranscriber();
        new T GetGene();
    }
}