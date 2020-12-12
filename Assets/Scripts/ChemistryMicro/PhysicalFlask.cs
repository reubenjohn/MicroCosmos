using System.Collections.Generic;
using Chemistry;
using UnityEngine;

namespace ChemistryMicro
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PhysicalFlask : MonoBehaviour, IFlaskBehavior<Substance>
    {
        protected readonly Flask<Substance> flask = new Flask<Substance>();
        private Rigidbody2D rb;

        private void Awake() => rb = GetComponent<Rigidbody2D>();

        private void OnDestroy()
        {
            if (!Mathf.Approximately(TotalMass, 0))
                Debug.LogWarning($"Attempting to destroy a flask of game object '{gameObject.name}' " +
                                 $" with non-zero mass of {TotalMass} violates the conservation of mass!");
        }

        public float TotalMass => flask.TotalMass;

        public float this[Substance substance] => flask[substance];
        public int Length => flask.Length;

        public Mixture<Substance> ToMixture() => flask.Copy();

        public void Convert(Reaction<Substance> reaction, float conversionFactor = 1f) =>
            flask.Convert(reaction, conversionFactor);

        public void TransferTo(PhysicalFlask destination, Mixture<Substance> mix)
        {
            Flask<Substance>.Transfer(destination.flask, flask, mix);
            OnReflectPhysicalProperties();
            destination.OnReflectPhysicalProperties();
        }

        public void LoadFlask(Dictionary<Substance, float> newFlaskContents)
        {
            var source = new Flask<Substance>(newFlaskContents);
            Flask<Substance>.Transfer(flask, source, source - flask);
            OnReflectPhysicalProperties();
            if (source.TotalMass > 0)
                Debug.LogWarning($"'{gameObject.name}' destroying {source.TotalMass}kg while loading");
        }

        private void OnReflectPhysicalProperties()
        {
            rb.mass = TotalMass;
            transform.localScale = Vector3.one * Mathf.Sqrt(rb.mass / RecipeBook.Density);
        }
    }
}