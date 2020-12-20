using System;
using System.IO;
using Brains;
using Cinematics;
using Genealogy.Graph;
using Genealogy.Layout.Asexual;
using Genealogy.Persistence;
using Genealogy.Visualization;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Environment
{
    [RequireComponent(typeof(CellColony), typeof(DivinePossession))]
    public class GenealogyGraphManager : MonoBehaviour, IGraphViewerListener, ICellColonyListener
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

            CellColony.AddListener(this);

            genealogyGraph.RegisterRootNode(
                new CellNode(Guid.Parse("00000000-0000-0000-0000-000000000000"), new DateTime(1), "Cell"));
        }

        private void OnDestroy() => stenographer.CloseScroll();

        public void OnSave(string saveDirectory) => stenographer.SaveCopy(PersistenceFilePath(saveDirectory));

        public void OnLoad(string saveDirectory)
        {
            genealogyGraph.Clear();
            using (var sw = new JsonTextReader(new StreamReader(PersistenceFilePath(saveDirectory))))
            {
                ScrollReader.Load(sw, genealogyGraph);
            }
        }

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

        public CellNode RegisterAsexualCellBirth(Node[] parentGenealogyNodes)
        {
            var displayName = NamingSystem.GetChildName(genealogyGraph, (CellNode) parentGenealogyNodes[0]);
            var genealogyNode = new CellNode(displayName);
            genealogyGraph.RegisterReproductionAndOffspring(parentGenealogyNodes, genealogyNode);
            return genealogyNode;
        }

        public string PersistenceFilePath(string saveDirectory) => $"{saveDirectory}/{saveFile}.json";

        public void RegisterDeath(CellNode cellNode) =>
            genealogyGraph.RegisterDeath(cellNode, new CellDeath(Guid.NewGuid(), DateTime.Now));
    }
}