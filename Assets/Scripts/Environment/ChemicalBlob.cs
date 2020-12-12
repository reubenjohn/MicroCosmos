using Chemistry;
using ChemistryMicro;
using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ChemicalBlob : PhysicalFlask
    {
        public static readonly double MinBlobSize = .01f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("ChemicalBlob"))
            {
                var otherBlob = other.gameObject.GetComponent<ChemicalBlob>();
                if (TotalMass < otherBlob.TotalMass ||
                    Mathf.Approximately(TotalMass, otherBlob.TotalMass) &&
                    gameObject.GetInstanceID() < other.GetInstanceID())
                {
                    otherBlob.Transfer(this, ToMixture());
                    Destroy(gameObject);
                }
            }
        }

        public static ChemicalBlob InstantiateBlob(PhysicalFlask source, Mixture<Substance> mix,
            Vector3 dumpSite, Transform parent)
        {
            var obj = Instantiate(Resources.Load<GameObject>("Objects/ChemicalBlob"),
                dumpSite, Quaternion.identity, parent);
            var blob = obj.GetComponent<ChemicalBlob>();
            blob.Transfer(source, mix);
            return blob;
        }
    }
}