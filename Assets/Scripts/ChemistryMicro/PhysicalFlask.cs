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

        protected virtual void OnDestroy()
        {
            if (!Mathf.Approximately(TotalMass, 0))
                Debug.LogWarning($"Attempting to destroy a flask of component '{GetType().Name}'" +
                                 $" of game object '{gameObject.name}' " +
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

        protected virtual void OnReflectPhysicalProperties()
        {
            rb.mass = TotalMass;
            transform.localScale = Vector3.one * Mathf.Sqrt(rb.mass / RecipeBook.Density);
        }
    }
}