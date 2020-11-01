using System.Linq;
using Genetics;
using Newtonsoft.Json;
using UnityEngine;

namespace Persistence
{
    [JsonConverter(typeof(GeneNodeJsonDeserializer))]
    public class GeneNode
    {
        public string resource;
        public string name;
        public object gene;
        public GeneNode[] children;

        [JsonIgnore] public ILivingComponent livingComponent;


        public static GeneNode Save(ILivingComponent livingComponent)
        {
            return new GeneNode
            {
                resource = livingComponent.GetResourcePath(),
                name = livingComponent.GetNodeName(),
                gene = livingComponent.GetGene(),
                children = livingComponent.GetSubLivingComponents()
                    .Select(Save)
                    .ToArray()
            };
        }

        public static GameObject Load(GeneNode geneNode, Transform container, Vector3 position, Quaternion rotation)
        {
            var gameObject =
                (GameObject) Object.Instantiate(Resources.Load(geneNode.resource), position, rotation, container);
            Load(geneNode, gameObject);
            return gameObject;
        }

        public static GameObject Load(GeneNode geneNode, Transform container)
        {
            var gameObject = (GameObject) Object.Instantiate(Resources.Load(geneNode.resource), container);
            Load(geneNode, gameObject);
            return gameObject;
        }

        private static void Load(GeneNode geneNode, GameObject newlyInstantiatedTarget)
        {
            var gameObject = newlyInstantiatedTarget;
            var livingComponent = gameObject.GetComponent<ILivingComponent>();
            gameObject.name = geneNode.name;
            var subLivingComponentContainer = livingComponent.OnInheritGene(geneNode.gene);
            foreach (var subGeneNode in geneNode.children)
            {
                var subObject =
                    (GameObject) Object.Instantiate(Resources.Load(subGeneNode.resource), subLivingComponentContainer);
                Load(subGeneNode, subObject);
            }
        }

        public static GeneNode GetMutated(ILivingComponent livingComponent)
        {
            return new GeneNode
            {
                resource = livingComponent.GetResourcePath(),
                name = livingComponent.GetNodeName(),
                gene = livingComponent.GetGeneTranscriber().Mutate(livingComponent.GetGene()),
                children = livingComponent.GetSubLivingComponents()
                    .Select(GetMutated)
                    .ToArray()
            };
        }
    }
}