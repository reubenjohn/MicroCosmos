using System;
using Chemistry;
using ChemistryMicro;
using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ChemicalBlob : MonoBehaviour
    {
        public static readonly double MinBlobSize = .01f;
        private readonly Flask<Substance> flask = new Flask<Substance>();
        private Rigidbody2D rb;
        public float TotalMass { get; private set; }

        private void Awake() => rb = GetComponent<Rigidbody2D>();

        private void OnDestroy()
        {
            if (!Mathf.Approximately(TotalMass, 0))
                throw new InvalidOperationException(
                    $"Attempting to destroy a flask host with non-zero mass of {TotalMass} " +
                    "violates the conservation of mass!");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("ChemicalBlob"))
            {
                var otherBlob = other.gameObject.GetComponent<ChemicalBlob>();
                if (TotalMass < otherBlob.TotalMass ||
                    Mathf.Approximately(TotalMass, otherBlob.TotalMass) &&
                    gameObject.GetInstanceID() < other.GetInstanceID())
                {
                    otherBlob.Transfer(this, flask);
                    Destroy(gameObject);
                }
            }
        }

        public static ChemicalBlob InstantiateBlob(Flask<Substance> source, Mixture<Substance> mix,
            Vector3 dumpSite, Transform parent)
        {
            var obj = Instantiate(Resources.Load<GameObject>("Objects/ChemicalBlob"),
                dumpSite, Quaternion.identity, parent);
            var blob = obj.GetComponent<ChemicalBlob>();
            blob.Transfer(source, mix);
            return blob;
        }

        public void Transfer(Flask<Substance> source, Mixture<Substance> mix)
        {
            Flask<Substance>.Transfer(flask, source, mix);
            TotalMass = flask.TotalMass;
            rb.mass = TotalMass;
            transform.localScale = Vector3.one * Mathf.Sqrt(rb.mass / RecipeBook.Density);
        }

        private void Transfer(ChemicalBlob source, Mixture<Substance> mix)
        {
            Transfer(source.flask, mix);
            source.TotalMass = source.flask.TotalMass;
        }
    }
}