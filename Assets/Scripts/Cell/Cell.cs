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

        public Transform OnInheritGene(CellGene inheritedGene)
        {
            var organellesTransform = transform;
            foreach (Transform existingSubTransforms in organellesTransform) Destroy(existingSubTransforms.gameObject);

            return organellesTransform;
        }

        public GeneTranscriber<CellGene> GetGeneTranscriber() => CellGeneTranscriber.Singleton;

        Transform ILivingComponent.OnInheritGene(object inheritedGene) => OnInheritGene((CellGene) inheritedGene);

        IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();

        public CellGene GetGene()
        {
            return new CellGene();
        }

        object ILivingComponent.GetGene() => ((ILivingComponent<CellGene>) this).GetGene();

        public string GetResourcePath() => "Cells/Cell1";

        public JObject GetState()
        {
            var state = new JObject
            {
                ["position"] = Serialization.ToSerializable(transform.position),
                ["rotation"] = transform.rotation.eulerAngles.z
            };
            return state;
        }

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
            var tran = transform;
            GeneNode.Load(geneTree, tran.parent, tran.position - tran.up * .3f, tran.rotation);
        }
    }
}