using System;
using Chemistry;
using UnityEngine;

namespace ChemistryMicro
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PhysicalFlask : MonoBehaviour, IFlaskBehavior<Substance>
    {
        protected readonly Flask<Substance> flask = new Flask<Substance>();
        private bool isFlaskFrozen;
        protected Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            OnReflectPhysicalProperties();
        }

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

        private void Freeze()
        {
            if (flask.TotalMass > 0)
                throw new InvalidOperationException("Cannot freeze a physical flask that has mass");
            isFlaskFrozen = true;
        }

        public Mixture<Substance> ToMixture() => flask.Copy();
        protected MixtureDictionary<Substance> ToMixtureDictionary() => flask.ToMixtureDictionary();

        private void AssertNotFrozen()
        {
            if (isFlaskFrozen) throw new InvalidOperationException("Cannot modify a flask once the flask is frozen");
        }

        public void Convert(Reaction<Substance> reaction, float conversionFactor = 1f)
        {
            AssertNotFrozen();
            flask.Convert(reaction, conversionFactor);
        }

        public void TransferTo(PhysicalFlask destination, Mixture<Substance> mix)
        {
            destination.AssertNotFrozen();
            AssertNotFrozen();
            Flask<Substance>.Transfer(destination.flask, flask, mix);
            OnReflectPhysicalProperties();
            destination.OnReflectPhysicalProperties();
        }

        public void MergeInto(PhysicalFlask destination)
        {
            if (this == null)
                return;
            try
            {
                TransferTo(destination, flask);
                Freeze();
            }
            finally
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnReflectPhysicalProperties()
        {
            rb.mass = TotalMass;
            transform.localScale = Vector3.one * (Mathf.Sqrt(rb.mass / RecipeBook.Density) / Mathf.PI);
        }

        public static void WithInterFlaskTransferLock(Action action)
        {
            action.Invoke();
        }
    }
}