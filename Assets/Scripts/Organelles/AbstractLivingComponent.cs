using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Organelles
{
    public abstract class AbstractLivingComponent<T> : MonoBehaviour, ILivingComponent<T>
    {
        public T gene;

        public string GetNodeName() => gameObject.name;

        Transform ILivingComponent.OnInheritGene(object inheritedGene) => OnInheritGene((T) inheritedGene);

        public abstract GeneTranscriber<T> GetGeneTranscriber();

        public T GetGene() => gene;

        public virtual Transform OnInheritGene(T inheritedGene)
        {
            gene = inheritedGene;
            return transform;
        }

        IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();

        object ILivingComponent.GetGene() => GetGene();

        public abstract string GetResourcePath();

        public virtual JObject GetState() => new JObject();

        public virtual void SetState(JObject state)
        {
        }

        public virtual ILivingComponent[] GetSubLivingComponents() => new ILivingComponent[0];
    }
}