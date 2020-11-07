using System.Linq;
using Genetics;
using Newtonsoft.Json.Linq;
using Persistence;
using UnityEngine;

namespace Cell
{
    public class Cell : MonoBehaviour, ILivingComponent<CellGene>
    {
        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public string GetNodeName() => gameObject.name;

        public Transform OnInheritGene(CellGene inheritedGene) => transform.DestroyChildren();

        public GeneTranscriber<CellGene> GetGeneTranscriber() => CellGeneTranscriber.Singleton;

        Transform ILivingComponent.OnInheritGene(object inheritedGene) => OnInheritGene((CellGene) inheritedGene);

        IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();

        public CellGene GetGene() => new CellGene();

        object ILivingComponent.GetGene() => ((ILivingComponent<CellGene>) this).GetGene();

        public string GetResourcePath() => "Cells/Cell1";

        public JObject GetState() =>
            new JObject
            {
                ["position"] = Serialization.ToSerializable(transform.position),
                ["rotation"] = transform.rotation.eulerAngles.z
            };

        public void SetState(JObject state)
        {
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
            }

            var position = state["position"];
            transform.position = position != null ? Serialization.ToVector2((string) position) : new Vector2();
            var rotation = state["rotation"];
            transform.rotation = rotation != null ? Quaternion.Euler(0, 0, (float) rotation) : new Quaternion();
        }

        public ILivingComponent[] GetSubLivingComponents()
        {
            return transform.Children()
                .Select(organelleTransform => organelleTransform.GetComponent<ILivingComponent>())
                .Where(e => e != null)
                .ToArray();
        }

        public void GiveBirth()
        {
            var geneTree = GeneNode.GetMutated(this);
            var t = transform;
            GeneNode.Load(geneTree, t.parent, t.position - t.up * .3f, t.rotation);
        }
    }
}