﻿using System.Linq;
using Genealogy;
using Genetics;
using Newtonsoft.Json.Linq;
using Organelles;
using UnityEngine;

namespace Cell
{
    public class Cell : AbstractLivingComponent<CellGene>
    {
        private Rigidbody2D rb;
        public GenealogyGraphManager genealogyGraphManager { get; private set; }

        public CellNode genealogyNode;
        public bool IsInFocus { get; set; }

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            genealogyGraphManager = GetComponentInParent<GenealogyGraphManager>();

            gene = gene ?? new CellGene();

            if (genealogyNode == null && genealogyGraphManager != null)
                genealogyGraphManager.RegisterAsexualCellBirth(new Node[] { }, this);
        }

        public override Transform OnInheritGene(CellGene inheritedGene) => transform.DestroyChildren();

        public override GeneTranscriber<CellGene> GetGeneTranscriber() => CellGeneTranscriber.Singleton;

        public override string GetResourcePath() => "Cells/Cell1";

        public override JObject GetState() =>
            new JObject
            {
                ["position"] = Serialization.ToSerializable(transform.position),
                ["rotation"] = transform.rotation.eulerAngles.z
            };

        public override void SetState(JObject state)
        {
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
            }

            var position = state["position"];
            transform.position = position != null ? Serialization.ToVector2((string) position) : new Vector2();
            var rotation = state["rotation"];
            transform.rotation = rotation != null ? Quaternion.Euler(0, 0, (float) rotation) : new Quaternion();
        }

        public override ILivingComponent[] GetSubLivingComponents()
        {
            return transform.Children()
                .Select(organelleTransform => organelleTransform.GetComponent<ILivingComponent>())
                .Where(e => e != null)
                .ToArray();
        }
    }
}