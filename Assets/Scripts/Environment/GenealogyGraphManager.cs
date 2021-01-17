using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Brains;
using Cell;
using Cinematics;
using Genealogy;
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
        public readonly GenealogyGraph genealogyGraph = new GenealogyGraph();
        private readonly List<ICellSelectionListener> listeners = new List<ICellSelectionListener>();
        private DivinePossession divinePossession;
        private LayoutManager layoutManager;
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

                layoutManager = new LayoutManager();
                layoutManager.AddListener(viewer);
                genealogyGraph.AddListener(layoutManager);
                genealogyGraph.AddListener(new DeadNodePruner());
                viewer.AddListener(this);

                AddCellSelectionListener(divinePossession);
                AddCellSelectionListener(choreographer);
            }
            else
            {
                Debug.LogWarning("No genealogy graph viewer is listening in on the genealogy graph");
            }

            genealogyGraph.RegisterRootNode(
                new CellNode(Guid.Parse("00000000-0000-0000-0000-000000000000"), new DateTime(1), "Cell"));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                var visibility = viewer.ToggleVisibility();
                layoutManager.LiveLayoutEnabled = visibility;
            }
        }

        private void OnDestroy() => stenographer.CloseScroll();

        public void OnSelectNode(ViewerNode viewerNode, PointerEventData eventData)
        {
            if (viewerNode.GenealogyNode.NodeType == NodeType.Cell)
            {
                var targetCell = CellColony.FindCell(viewerNode.GenealogyNode.Guid);
                if (targetCell != null)
                {
                    foreach (var listener in listeners)
                        listener.OnCellSelectionChange(targetCell, true);
                    targetCell.IsInFocus = true;
                }
            }
        }

        public void OnDeselectNode(ViewerNode viewerNode, PointerEventData eventData)
        {
            if (viewerNode.GenealogyNode.NodeType == NodeType.Cell)
            {
                var targetCell = CellColony.FindCell(viewerNode.GenealogyNode.Guid);
                if (targetCell != null)
                {
                    foreach (var listener in listeners)
                        listener.OnCellSelectionChange(targetCell, false);
                    targetCell.IsInFocus = false;
                }
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

        private void AddCellSelectionListener(ICellSelectionListener listener) => listeners.Add(listener);

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