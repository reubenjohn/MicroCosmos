using System;
using System.Linq;
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
        public GenealogyGraphManager GenealogyGraphManager { get; private set; }

        private CellNode genealogyNode;

        public CellNode GenealogyNode
        {
            get => genealogyNode;
            set
            {
                if (genealogyNode != null)
                    throw new InvalidOperationException($"Cell.GenealogyNode is already set to {GenealogyNode}");
                genealogyNode = value;
            }
        }

        public bool IsInFocus { get; set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            GenealogyGraphManager = GetComponentInParent<GenealogyGraphManager>();
        }

        public override CellGene GetGene() => new CellGene();

        public override Transform OnInheritGene(CellGene inheritedGene) => transform.DestroyChildren();

        public override GeneTranscriber<CellGene> GetGeneTranscriber() => CellGeneTranscriber.Singleton;

        public override string GetResourcePath() => "Cells/Cell1";

        public override JObject GetState()
        {
            var jObject = new JObject
            {
                ["position"] = Serialization.ToSerializable(transform.position),
                ["rotation"] = transform.rotation.eulerAngles.z
            };
            if (GenealogyNode != null)
                jObject["guid"] = GenealogyNode.Guid.ToString();
            return jObject;
        }

        public override void SetState(JObject state)
        {
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0;
            }

            var guid = (string) state["guid"];
            if (guid != null && GenealogyGraphManager != null)
            {
                GenealogyNode = (CellNode) GenealogyGraphManager.genealogyGraph.GetNodeOrDefault(Guid.Parse(guid)) ??
                                GenealogyGraphManager.RegisterAsexualCellBirth(
                                    new[] {GenealogyGraphManager.RootNode}, this);
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