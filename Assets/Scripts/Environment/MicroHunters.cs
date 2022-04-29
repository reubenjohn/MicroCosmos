using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Brains.HunterBrain;
using Genealogy.Graph;
using UnityEngine;

namespace Environment
{
    public class MicroHunters : MonoBehaviour
    {
        private static GenealogyGraphManager graphManager;
        public static HunterBrain[] Hunters { get; private set; }
        public static Cell.Cell TeamABase { get; set; }
        public static Cell.Cell TeamBBase { get; set; }
        public static IEnumerable<HunterBrain> TeamAHunters { get; private set; }
        public static IEnumerable<HunterBrain> TeamBHunters { get; private set; }


        private void Start()
        {
            graphManager = GetComponentInChildren<GenealogyGraphManager>();
            Hunters = Array.Empty<HunterBrain>();
            TeamAHunters = Array.Empty<HunterBrain>();
            TeamBHunters = Array.Empty<HunterBrain>();
            StartCoroutine(StartStatsPlotting());
        }

        private IEnumerator StartStatsPlotting()
        {
            while (true)
            {
                Hunters = GetComponentsInChildren<HunterBrain>();
                TeamAHunters = Hunters.Where(hunter => GetHunterTeam(ClassifyCell(hunter.cell)) == HunterTeam.TeamA);
                TeamBHunters = Hunters.Where(hunter => GetHunterTeam(ClassifyCell(hunter.cell)) == HunterTeam.TeamB);
                foreach (var hunterBrain in Hunters)
                    hunterBrain.ComputeAndCacheElectrostaticForce();
                yield return new WaitForSeconds(.5f);
            }
        }

        public static CellClassification ClassifyCell(Cell.Cell cell)
        {
            var genealogyNode = cell.GenealogyNode;
            if (genealogyNode.Guid == SimParams.Singleton.hunterBaseAGuid)
                return CellClassification.BaseA;
            if (genealogyNode.Guid == SimParams.Singleton.hunterBaseBGuid)
                return CellClassification.BaseB;
            var parentNode = GetAsexualParentGenealogyNode(genealogyNode);
            if (parentNode.Guid == SimParams.Singleton.hunterBaseAGuid)
                return CellClassification.HunterA;
            if (parentNode.Guid == SimParams.Singleton.hunterBaseBGuid)
                return CellClassification.HunterB;
            return CellClassification.Sheep;
        }

        private static Node GetAsexualParentGenealogyNode(Node node)
        {
            if (graphManager == null)
                return null;
            var reproductionGuid = graphManager.genealogyGraph.GetRelationsTo(node.Guid)[0].From.Guid;
            return graphManager.genealogyGraph.GetRelationsTo(reproductionGuid)[0].From;
        }

        public static bool IsHomeBase(Cell.Cell cell) => GetCellType(ClassifyCell(cell)) == CellType.Base;

        public static CellType GetCellType(CellClassification classification) => ClassificationToType[classification];

        public static HunterTeam GetHunterTeam(CellClassification classification) =>
            ClassificationToTeam[classification];

        private static readonly Dictionary<CellClassification, CellType> ClassificationToType =
            new Dictionary<CellClassification, CellType>
            {
                [CellClassification.BaseA] = CellType.Base,
                [CellClassification.BaseB] = CellType.Base,
                [CellClassification.HunterA] = CellType.Hunter,
                [CellClassification.HunterB] = CellType.Hunter,
                [CellClassification.Sheep] = CellType.Sheep
            };

        private static readonly Dictionary<CellClassification, HunterTeam> ClassificationToTeam =
            new Dictionary<CellClassification, HunterTeam>
            {
                [CellClassification.BaseA] = HunterTeam.TeamA,
                [CellClassification.BaseB] = HunterTeam.TeamB,
                [CellClassification.HunterA] = HunterTeam.TeamA,
                [CellClassification.HunterB] = HunterTeam.TeamB,
                [CellClassification.Sheep] = HunterTeam.NA
            };
    }

    public enum CellType
    {
        Base,
        Hunter,
        Sheep
    }

    public enum HunterTeam
    {
        TeamA,
        TeamB,
        NA
    }

    public enum CellClassification
    {
        BaseA,
        BaseB,
        HunterA,
        HunterB,
        Sheep
    }
}