using Genetics;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Organelles
{
    public class FlagellaActuator : MonoBehaviour, IActuator, ILivingComponent<FlagellaGene>
    {
        public static readonly string ResourcePath = "Organelles/Flagella1";

        public FlagellaGene gene = new FlagellaGene(250f, 10f);
        public Rigidbody2D rb { get; private set; }

        private void Start() => rb = GetComponentInParent<Rigidbody2D>();

        public float[] Connect() => new float[2];

        public void Actuate(float[] logits)
        {
            if (GetComponentInParent<Cell.Cell>().gameObject == Selection.activeGameObject)
            {
                Grapher.Log(logits[0], "Flagella[0]", Color.blue);
                Grapher.Log(logits[1], "Flagella[1]", Color.cyan);   
            }
            rb.AddRelativeForce(CalculateRelativeForce(gene, logits, Time.deltaTime));
            rb.AddTorque(CalculateTorque(gene, logits, Time.deltaTime));
        }

        public static Vector2 CalculateRelativeForce(FlagellaGene gene, float[] logits, float deltaTime)
        {
            return logits[0] * gene.linearPower * deltaTime * Vector2.up;
        }

        public static float CalculateTorque(FlagellaGene gene, float[] logits, float deltaTime)
        {
            return logits[1] * gene.angularPower * deltaTime;
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