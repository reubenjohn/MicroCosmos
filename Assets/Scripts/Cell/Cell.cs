using System;
using System.Linq;
using Cinematics;
using Environment;
using Genealogy.Graph;
using Genetics;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Organelles;
using Organelles.CellCauldron;
using UnityEngine;
using Util;

namespace Cell
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CellCauldron))]
    public class Cell : AbstractLivingComponent<CellGene>
    {
        public static readonly string ResourcePath = "Cells/Cell1";
        public static readonly float MinMass = .01f;

        private CellNode genealogyNode;
        private Rigidbody2D rb;
        public CellCauldron Cauldron { get; private set; }
        private GenealogyGraphManager GenealogyGraphManager { get; set; }

        public CellNode GenealogyNode
        {
            get => genealogyNode;
            private set
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
            Cauldron = GetComponent<CellCauldron>();
            GenealogyGraphManager = GetComponentInParent<GenealogyGraphManager>();
        }

        private void Start() => HaloPopQueue.Instance.Enqueue(transform, Color.green, 1.5f);

        public override CellGene GetGene()
        {
            gene.cauldron = Cauldron.GetGene();
            return gene;
        }

        public override Transform OnInheritGene(CellGene inheritedGene)
        {
            gene = inheritedGene;
            Cauldron.OnInheritGene(inheritedGene.cauldron);
            return transform;
        }

        public override GeneTranscriber<CellGene> GetGeneTranscriber() => CellGeneTranscriber.Singleton;

        public override string GetResourcePath() => ResourcePath;

        public override JObject GetState()
        {
            var t = transform;
            var jObject = GetState(GenealogyNode.Guid, t.position, t.rotation, Cauldron.GetState());
            return jObject;
        }

        public static JObject GetState(Guid guid, Vector3 position, Quaternion rotation, [NotNull] JObject cauldron) =>
            new JObject
            {
                ["guid"] = guid.ToString(),
                ["position"] = Serialization.ToSerializable(position),
                ["rotation"] = rotation.eulerAngles.z,
                ["cauldron"] = cauldron
            };

        public override void SetState(JObject state)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            var position = state["position"];
            transform.position = position != null ? Serialization.ToVector2((string) position) : new Vector2();
            var rotation = state["rotation"];
            transform.rotation = rotation != null ? Quaternion.Euler(0, 0, (float) rotation) : new Quaternion();

            Cauldron.SetState((JObject) state["cauldron"]);

            var guid = (string) state["guid"];
            GenealogyNode = (CellNode) GenealogyGraphManager.genealogyGraph.GetNode(Guid.Parse(guid));
            name = GenealogyNode.displayName;
        }

        public override ILivingComponent[] GetSubLivingComponents() =>
            transform.Children()
                .Select(organelleTransform => organelleTransform.GetComponent<ILivingComponent>())
                .Where(e => e != null)
                .ToArray();

        public void Die()
        {
            HaloPopQueue.Instance.Enqueue(transform, Color.red, 2f);
            Cauldron.DestroyConservatively();
            if (GenealogyGraphManager != null)
                GenealogyGraphManager.RegisterDeath(genealogyNode);
            if (Cauldron.TotalMass > 0)
                Debug.LogError($"Cauldron should be empty on dying, but has mass of {Cauldron.TotalMass}!");
            Destroy(gameObject);
        }
    }
}