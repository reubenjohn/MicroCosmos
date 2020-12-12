using System;
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
                throw new InvalidOperationException(
                    $"Attempting to destroy a flask host with non-zero mass of {TotalMass} " +
                    "violates the conservation of mass!");
        }

        public float TotalMass => flask.TotalMass;

        public float this[Substance substance] => flask[substance];
        public int Length => flask.Length;

        public Mixture<Substance> ToMixture() => flask.Copy();

        protected void Convert(Reaction<Substance> reaction, float conversionFactor = 1f) =>
            flask.Convert(reaction, conversionFactor);

        public void Transfer(PhysicalFlask source, Mixture<Substance> mix)
        {
            Flask<Substance>.Transfer(flask, source.flask, mix);
            OnReflectPhysicalProperties();
            source.OnReflectPhysicalProperties();
        }

        protected void LoadFlask(Dictionary<Substance, float> newFlaskContents)
        {
            var source = new Flask<Substance>(newFlaskContents);
            Flask<Substance>.Transfer(flask, source, source - flask);
            OnReflectPhysicalProperties();
            if (source.TotalMass > 0)
                Debug.LogWarning($"Destroying {source.TotalMass}kg while inheriting gene");
        }

        private void OnReflectPhysicalProperties()
        {
            rb.mass = TotalMass;
            transform.localScale = Vector3.one * Mathf.Sqrt(rb.mass / RecipeBook.Density);
        }
    }
}