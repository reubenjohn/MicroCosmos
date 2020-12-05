using System;
using System.IO;
using Brains;
using Cinematics;
using Genealogy;
using Genealogy.Asexual;
using Genealogy.Persistence;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cell
{
    [RequireComponent(typeof(CellColony))]
    public class GenealogyGraphManager : MonoBehaviour, IGraphViewerListener, ICellColonyListener
    {
        [SerializeField] private Choreographer Choreographer;
        [SerializeField] private GenealogyGraphViewer viewer;
        public readonly GenealogyGraph genealogyGraph = new GenealogyGraph();
        private DivinePossession divinePossession;
        private ScrollStenographer stenographer;
        private string stenographerPath;
        private CellColony CellColony { get; set; }

        public Node RootNode => genealogyGraph.rootNode;

        private void Start()
        {
            CellColony = GetComponent<CellColony>();
            divinePossession = GetComponent<DivinePossession>();

            stenographerPath = $"{CellColony.SaveDirectory}/{Guid.NewGuid()}.json";
            stenographer = new ScrollStenographer(() => new JsonTextWriter(new StreamWriter(stenographerPath)));
            genealogyGraph.AddListener(stenographer);

            if (viewer)
            {
                Choreographer.AddListener(viewer);

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

        private void OnDestroy()
        {
            stenographer.CloseScroll();
        }

        public void OnSave(string saveDirectory)
        {
            stenographer.CloseScroll();
            var destFileName = PersistenceFilePath(saveDirectory);
            File.Delete(destFileName);
            File.Move(stenographerPath, destFileName);
        }

        public void OnLoad(string saveDirectory)
        {
            genealogyGraph.Clear();
            ScrollReader.Load(new JsonTextReader(new StreamReader(PersistenceFilePath(saveDirectory))),
                genealogyGraph);
        }

        public void OnSelectNode(ViewerNode viewerNode, PointerEventData eventData)
        {
            if (viewerNode.GenealogyNode.NodeType == NodeType.Cell)
            {
                var targetCell = CellColony.FindCell(viewerNode.GenealogyNode.Guid);
                if (targetCell != null)
                {
                    Choreographer.SetFocus(targetCell.gameObject);
                    divinePossession.SetPossessionTarget(targetCell);
                    targetCell.IsInFocus = true;
                }
            }
        }

        public void OnDeselectNode(ViewerNode viewerNode, PointerEventData eventData)
        {
            if (viewerNode.GenealogyNode.NodeType == NodeType.Cell)
            {
                Choreographer.SetFocus(null);
                divinePossession.SetPossessionTarget(null);
                var targetCell = CellColony.FindCell(viewerNode.GenealogyNode.Guid);
                if (targetCell != null)
                    targetCell.IsInFocus = false;
            }
        }

        public CellNode RegisterAsexualCellBirth(Node[] parentGenealogyNodes, Cell child)
        {
            child.name = NamingSystem.GetChildName(genealogyGraph, (CellNode) parentGenealogyNodes[0]);
            var childGenealogyNode = new CellNode(child.name);
            genealogyGraph.RegisterReproductionAndOffspring(parentGenealogyNodes, childGenealogyNode);
            return childGenealogyNode;
        }

        private static string PersistenceFilePath(string saveDirectory)
        {
            return $"{saveDirectory}/genealogy1.json";
        }
    }
}