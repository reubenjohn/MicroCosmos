using System.Linq;
using Genetics;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Persistence
{
    [NoReorder]
    [JsonConverter(typeof(GeneNodeJsonDeserializer))]
    public class GeneNode // TODO Rename to GeneTree
    {
        public readonly string resource;
        public readonly object gene;
        public GeneNode[] children;

        [JsonIgnore] public readonly ILivingComponent livingComponent;

        public GeneNode(ILivingComponent livingComponent, object gene, GeneNode[] children)
        {
            resource = livingComponent.GetResourcePath();
            this.gene = gene;
            this.children = children;
            this.livingComponent = livingComponent;
        }


        public static GeneNode Save(ILivingComponent livingComponent) => // TODO Rename to From
            new GeneNode(
                livingComponent,
                livingComponent.GetGene(),
                livingComponent.GetSubLivingComponents()
                    .Select(Save)
                    .ToArray()
            );

        // TODO Rename to Instantiate
        public static GameObject Load(GeneNode geneNode, Transform container, Vector3 position, Quaternion rotation)
        {
            var gameObject = (GameObject) Object.Instantiate(Resources.Load(geneNode.resource),
                position, rotation, container);
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
            var subLivingComponentContainer = livingComponent.OnInheritGene(geneNode.gene);
            foreach (var subGeneNode in geneNode.children)
            {
                var subObject =
                    (GameObject) Object.Instantiate(Resources.Load(subGeneNode.resource), subLivingComponentContainer);
                Load(subGeneNode, subObject);
            }
        }

        public static GeneNode GetMutated(ILivingComponent livingComponent) =>
            livingComponent.GetGeneTranscriber().GetTreeMutator().GetMutated(Save(livingComponent));
    }
}