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

        protected virtual void Awake()
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

        private bool FrozenCheck()
        {
            if (isFlaskFrozen)
                Debug.LogWarning("Skipping requested flask modification since the flask is already frozen.");
            return isFlaskFrozen;
        }

        public void Convert(Reaction<Substance> reaction, float conversionFactor = 1f)
        {
            if (FrozenCheck()) return;
            flask.Convert(reaction, conversionFactor);
            OnReflectPhysicalProperties();
        }

        public void TransferTo(PhysicalFlask destination, Mixture<Substance> mix)
        {
            if (destination.FrozenCheck() || FrozenCheck()) return;
            Flask<Substance>.Transfer(destination.flask, flask, mix);
            OnReflectPhysicalProperties();
            destination.OnReflectPhysicalProperties();
        }

        public void MergeInto(PhysicalFlask destination)
        {
            if (this == null || destination.FrozenCheck() || FrozenCheck()) return;
            try
            {
                Flask<Substance>.Transfer(destination.flask, flask, flask);
                destination.OnReflectPhysicalProperties();
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