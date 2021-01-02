using System.Collections;
using UnityEngine;

namespace Environment
{
    public class Environment : MonoBehaviour
    {
        private int ChemicalBlobCount { get; set; }
        public int CellCount { get; private set; }


        private void Start()
        {
            StartCoroutine(StartStatsPlotting());
        }

        private IEnumerator StartStatsPlotting()
        {
            while (true)
            {
                CellCount = GetComponentsInChildren<Cell.Cell>().Length;
                ChemicalBlobCount = GetComponentsInChildren<ChemicalBlob>().Length;
                Grapher.Log(CellCount, "Cell Count");
                Grapher.Log(ChemicalBlobCount, "Chemical Blob Count");
                yield return new WaitForSeconds(2);
            }

            // ReSharper disable once IteratorNeverReturns
        }
    }
}