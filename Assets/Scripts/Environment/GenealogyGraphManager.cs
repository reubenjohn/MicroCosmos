using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Brains;
using Cinematics;
using Genealogy.Graph;
using Genealogy.Layout.Asexual;
using Genealogy.Persistence;
using Genealogy.Visualization;
using Newtonsoft.Json;
using Persistence;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Environment
{
    [RequireComponent(typeof(CellColony), typeof(DivinePossession))]
    public class GenealogyGraphManager : MonoBehaviour, IGraphViewerListener,
        ISavableSubsystem<GenealogyScrollEntryBase>
    {
        [FormerlySerializedAs("Choreographer")] [SerializeField]
        private Choreographer choreographer;

        [SerializeField] private GenealogyGraphViewer viewer;
        public string saveFile = "genealogy1";
        public readonly GenealogyGraph genealogyGraph = new GenealogyGraph();
        private DivinePossession divinePossession;
        private ScrollStenographer stenographer;
        private CellColony CellColony { get; set; }

        public Node RootNode => genealogyGraph.rootNode;

        private void Start()
        {
            CellColony = GetComponent<CellColony>();
            divinePossession = GetComponent<DivinePossession>();

            stenographer = new ScrollStenographer();
            genealogyGraph.AddListener(stenographer);

            if (viewer)
            {
                if (choreographer) choreographer.AddListener(viewer);

                var layoutManager = new LayoutManager();
                layoutManager.AddListener(viewer);
                genealogyGraph.AddListener(layoutManager);
                viewer.AddListener(this);
            }
            else
            {
                Debug.LogWarning("No genealogy graph viewer is listening in on the genealogy graph");
            }

            genealogyGraph.RegisterRootNode(
                new CellNode(Guid.Parse("00000000-0000-0000-0000-000000000000"), new DateTime(1), "Cell"));
        }

        private void OnDestroy() => stenographer.CloseScroll();

        public void OnSelectNode(ViewerNode viewerNode, PointerEventData eventData)
        {
            if (viewerNode.GenealogyNode.NodeType == NodeType.Cell)
            {
                var targetCell = CellColony.FindCell(viewerNode.GenealogyNode.Guid);
                if (targetCell != null)
                {
                    if (choreographer) choreographer.SetFocus(targetCell.gameObject);
                    divinePossession.SetPossessionTarget(targetCell);
                    targetCell.IsInFocus = true;
                }
            }
        }

        public void OnDeselectNode(ViewerNode viewerNode, PointerEventData eventData)
        {
            if (viewerNode.GenealogyNode.NodeType == NodeType.Cell)
            {
                if (choreographer) choreographer.SetFocus(null);
                divinePossession.SetPossessionTarget(null);
                var targetCell = CellColony.FindCell(viewerNode.GenealogyNode.Guid);
                if (targetCell != null)
                    targetCell.IsInFocus = false;
            }
        }

        public string GetID() => typeof(GenealogyGraphManager).FullName;

        public int GetPersistenceVersion() => 1;

        public Type GetSavableType() => typeof(GenealogyScrollEntryBase);

        IEnumerable ISavableSubsystem.Save() => Save();
        public IEnumerable<GenealogyScrollEntryBase> Save() => stenographer.ReadAll();

        public void Load(IEnumerable save) => Load(save.Cast<GenealogyScrollEntryBase>());

        public JsonSerializer GetSerializer() =>
            new JsonSerializer
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = new NodeAsGuidContract()
            };

        public void Load(IEnumerable<GenealogyScrollEntryBase> save)
        {
            genealogyGraph.Clear();
            new ScrollReader(genealogyGraph).Load(save);
        }

        public CellNode RegisterAsexualCellBirth(Node[] parentGenealogyNodes)
        {
            var displayName = NamingSystem.GetChildName(genealogyGraph, (CellNode) parentGenealogyNodes[0]);
            var genealogyNode = new CellNode(displayName);
            genealogyGraph.RegisterReproductionAndOffspring(parentGenealogyNodes, genealogyNode);
            return genealogyNode;
        }

        public void RegisterDeath(CellNode cellNode) =>
            genealogyGraph.RegisterDeath(cellNode, new CellDeath(Guid.NewGuid(), DateTime.Now));
    }
}