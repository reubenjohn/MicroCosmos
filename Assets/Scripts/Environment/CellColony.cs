using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cell;
using ChemistryMicro;
using Genetics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Organelles.CellCauldron;
using Persistence;
using UnityEngine;

namespace Environment
{
    public class CellColony : MonoBehaviour, ISavableSubsystem<CellData>
    {
        private Cell.Cell[] LivingCells => transform.GetComponentsInChildren<Cell.Cell>();

        private void Start() => GetComponent<GenealogyGraphManager>();

        public string GetID() => typeof(CellColony).FullName;

        public int GetPersistenceVersion() => 2;

        public Type GetSavableType() => typeof(CellData);

        public JsonSerializer GetSerializer() => new JsonSerializer {Formatting = Formatting.Indented};

        IEnumerable ISavableSubsystem.Save() => Save();

        public IEnumerable<CellData> Save() => LivingCells.Select(ToCellData);

        public void Load(IEnumerable save) => Load(save.Cast<CellData>());

        public void Load(IEnumerable<CellData> save)
        {
            var sink = GetComponentInParent<ChemicalSink>();
            foreach (var cell in GetComponentsInChildren<Cell.Cell>())
            {
                cell.Cauldron.MergeInto(sink);
                Destroy(cell.gameObject);
            }

            foreach (var cellData in save)
                SpawnCell(cellData, sink);
        }

        private CellData ToCellData(Cell.Cell cell) =>
            new CellData
            {
                geneTree = GeneNode.Save(cell),
                stateTree = StateNode.Save(cell)
            };

        public GameObject SpawnCell(GeneNode geneTree, JObject cellState, PhysicalFlask sourceFlask)
        {
            var stateTree = StateNode.Empty(geneTree);
            stateTree.state = cellState;
            return SpawnCell(new CellData {geneTree = geneTree, stateTree = stateTree}, sourceFlask);
        }

        private GameObject SpawnCell(CellData cellData, PhysicalFlask sourceFlask)
        {
            var cellObj = GeneNode.Load(cellData.geneTree, transform);
            cellObj.GetComponent<CellCauldron>().SourceFlask = sourceFlask;
            StateNode.Load(cellObj.GetComponent<ILivingComponent>(), cellData.stateTree);
            return cellObj;
        }

        public Cell.Cell FindCell(Guid genealogyNodeGuid)
        {
            return LivingCells
                .FirstOrDefault(c => genealogyNodeGuid == c.GenealogyNode.Guid);
        }
    }
}