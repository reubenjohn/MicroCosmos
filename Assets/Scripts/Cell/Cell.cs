using System;
using System.Linq;
using Genealogy;
using Genetics;
using Newtonsoft.Json.Linq;
using Organelles;
using Organelles.ChemicalBag;
using UnityEngine;

namespace Cell
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CellCauldron))]
    public class Cell : AbstractLivingComponent<CellGene>
    {
        private CellCauldron cauldron;

        private CellNode genealogyNode;
        private Rigidbody2D rb;
        public GenealogyGraphManager GenealogyGraphManager { get; private set; }

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
            cauldron = GetComponent<CellCauldron>();
            GenealogyGraphManager = GetComponentInParent<GenealogyGraphManager>();
        }

        public override CellGene GetGene()
        {
            return new CellGene
            {
                cauldron = cauldron.GetGene()
            };
        }

        public override Transform OnInheritGene(CellGene inheritedGene)
        {
            cauldron.OnInheritGene(inheritedGene.cauldron);
            return transform;
        }

        public override GeneTranscriber<CellGene> GetGeneTranscriber()
        {
            return CellGeneTranscriber.Singleton;
        }

        public override string GetResourcePath()
        {
            return "Cells/Cell1";
        }

        public override JObject GetState()
        {
            var jObject = new JObject
            {
                ["position"] = Serialization.ToSerializable(transform.position),
                ["rotation"] = transform.rotation.eulerAngles.z,
                ["cauldron"] = cauldron.GetState()
            };
            if (GenealogyNode != null)
                jObject["guid"] = GenealogyNode.Guid.ToString();
            return jObject;
        }

        public override void SetState(JObject state)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            var position = state["position"];
            transform.position = position != null ? Serialization.ToVector2((string) position) : new Vector2();
            var rotation = state["rotation"];
            transform.rotation = rotation != null ? Quaternion.Euler(0, 0, (float) rotation) : new Quaternion();

            cauldron.SetState((JObject) state["cauldron"]);

            var guid = (string) state["guid"];
            if (guid != null && GenealogyGraphManager != null)
                GenealogyNode = (CellNode) GenealogyGraphManager.genealogyGraph.GetNodeOrDefault(Guid.Parse(guid)) ??
                                GenealogyGraphManager.RegisterAsexualCellBirth(
                                    new[] {GenealogyGraphManager.RootNode}, this);
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