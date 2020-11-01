using Genetics;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Actuators
{
    public class FlagellaActuator : MonoBehaviour, IActuator, ILivingComponent<FlagellaGene>
    {
        public static readonly string ResourcePath = "Organelles/Flagella1";

        public FlagellaGene gene = new FlagellaGene(250f, 10f);
        public Rigidbody2D rb { get; private set; }

        private void Start()
        {
            rb = GetComponentInParent<Rigidbody2D>();
        }

        public float[] Connect()
        {
            return new float[2];
        }

        public void Actuate(float[] logits)
        {
            Grapher.Log(logits[0], "Flagella[0]", Color.blue);
            Grapher.Log(logits[1], "Flagella[1]", Color.cyan);
            rb.AddRelativeForce(logits[0] * gene.linearPower * Time.deltaTime * Vector2.up);
            rb.AddTorque(logits[1] * gene.angularPower * Time.deltaTime);
        }

        public string GetNodeName() => gameObject.name;

        public Transform OnInheritGene(FlagellaGene inheritedGene)
        {
            gene = inheritedGene;
            return null;
        }

        Transform ILivingComponent.OnInheritGene(object inheritedGene) => OnInheritGene((FlagellaGene) inheritedGene);

        public GeneTranscriber<FlagellaGene> GetGeneTranscriber() => FlagellaGeneTranscriber.Singleton;
        IGeneTranscriber ILivingComponent.GetGeneTranscriber() => GetGeneTranscriber();

        public FlagellaGene GetGene() => gene;
        object ILivingComponent.GetGene() => GetGene();

        string ILivingComponent.GetResourcePath() => ResourcePath;

        public JObject GetState()
        {
            var dict = new JObject();
            // dict.Add("gene", GENE_TRANSCRIBER.Serialize(gene));
            return dict;
        }

        public void SetState(JObject state)
        {
        }

        public ILivingComponent[] GetSubLivingComponents() => new ILivingComponent[] { };
    }
}