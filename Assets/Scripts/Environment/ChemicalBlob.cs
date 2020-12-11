using Chemistry;
using ChemistryMicro;
using Organelles.CellCauldron;
using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ChemicalBlob : MonoBehaviour
    {
        public static readonly double MinBlobSize = .01f;
        private readonly Flask<Substance> flask = new Flask<Substance>();

        private ChemicalBagGene gene;

        private bool massUpdated = true;
        private Rigidbody2D rb;
        private float totalMass;

        private float TotalMass
        {
            get
            {
                if (massUpdated)
                {
                    massUpdated = false;
                    totalMass = flask.TotalMass;
                }

                return totalMass;
            }
        }

        public float this[Substance substance] => flask[substance];

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var chemicalBlob = other.gameObject.GetComponent<ChemicalBlob>();
            if (chemicalBlob != null &&
                (TotalMass < chemicalBlob.TotalMass ||
                 Mathf.Approximately(TotalMass, chemicalBlob.TotalMass) &&
                 gameObject.GetInstanceID() < other.GetInstanceID()))
            {
                chemicalBlob.Transfer(flask, flask);
                Destroy(gameObject);
            }
        }

        public static void InstantiateBlob(Flask<Substance> source, Mixture<Substance> mix, Vector3 dumpSite,
            Transform parent)
        {
            var obj = Instantiate(Resources.Load<GameObject>("Objects/ChemicalBlob"),
                dumpSite, Quaternion.identity, parent);
            var blob = obj.GetComponent<ChemicalBlob>();
            blob.Transfer(source, mix);
        }

        private void Transfer(Flask<Substance> source, Mixture<Substance> mix)
        {
            massUpdated = true;
            Flask<Substance>.Transfer(flask, source, mix);
            rb.mass = TotalMass;
            transform.localScale = Vector3.one * Mathf.Sqrt(rb.mass / RecipeBook.Density);
        }
    }
}