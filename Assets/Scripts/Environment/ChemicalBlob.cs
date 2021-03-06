﻿using Chemistry;
using ChemistryMicro;
using UnityEngine;
using Util;

namespace Environment
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ChemicalBlob : PhysicalFlask
    {
        public static readonly float MinBlobSize = .001f;
        private SpriteRenderer spriteRenderer;

        protected override void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            base.Awake();
        }

        private void Start() => name = $"ChemicalBlob{GetInstanceID()}";

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("ChemicalBlob"))
            {
                var otherBlob = other.gameObject.GetComponent<ChemicalBlob>();
                if (TotalMass < otherBlob.TotalMass ||
                    Mathf.Approximately(TotalMass, otherBlob.TotalMass) &&
                    gameObject.GetInstanceID() < other.GetInstanceID())
                    CoalesceInto(otherBlob);
            }
        }

        public void CoalesceInto(ChemicalBlob otherBlob)
        {
            MergeInto(otherBlob);
            otherBlob.rb.velocity = Vector2.zero;
        }

        protected override void OnReflectPhysicalProperties()
        {
            base.OnReflectPhysicalProperties();
            spriteRenderer.color = SubstanceColor.ColorOf(flask);
        }

        public static ChemicalBlob InstantiateBlob(PhysicalFlask source, Mixture<Substance> mix,
            Vector3 dumpSite, Transform parent)
        {
            var obj = Instantiate(Resources.Load<GameObject>("Objects/ChemicalBlob"),
                dumpSite, Quaternion.identity, parent);
            var blob = obj.GetComponent<ChemicalBlob>();
            source.TransferTo(blob, mix);
            return blob;
        }

        public static void InstantiateBlob(PhysicalFlask source, Vector3 dumpSite, Transform parent)
        {
            var obj = Instantiate(Resources.Load<GameObject>("Objects/ChemicalBlob"),
                dumpSite, Quaternion.identity, parent);
            var blob = obj.GetComponent<ChemicalBlob>();
            source.MergeInto(blob);
        }

        public static ChemicalBlobSave Save(ChemicalBlob blob) =>
            new ChemicalBlobSave(Serialization.ToSerializable(blob.transform.position),
                EnumUtils.ToNamedDictionary(blob.ToMixtureDictionary()));

        public static void Load(PhysicalFlask source, ChemicalBlobSave save, Transform parent) =>
            InstantiateBlob(source,
                new Mixture<Substance>(EnumUtils.ParseNamedDictionary<Substance, float>(save.Mixture)),
                Serialization.ToVector2(save.Position),
                parent
            );
    }
}