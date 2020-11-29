using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Genetics
{
    [RequireComponent(typeof(ILivingComponent))]
    public class GeneInjector : MonoBehaviour
    {
        public string gene;

        private void Start()
        {
            var livingComponent = GetComponent<ILivingComponent>();
            var geneObject = livingComponent.GetGeneTranscriber().Deserialize(JToken.Parse(gene));
            livingComponent.OnInheritGene(geneObject);
        }
    }
}