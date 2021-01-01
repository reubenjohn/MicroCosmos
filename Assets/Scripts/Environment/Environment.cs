using System.Collections;
using UnityEngine;

namespace Environment
{
    public class Environment : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(StartStatsPlotting());
        }

        private IEnumerator StartStatsPlotting()
        {
            while (true)
            {
                var cellCount = GetComponentsInChildren<Cell.Cell>().Length;
                var chemicalBlob = GetComponentsInChildren<ChemicalBlob>().Length;
                Grapher.Log(cellCount, "Cell Count");
                Grapher.Log(chemicalBlob, "Chemical Blob Count");
                yield return new WaitForSeconds(2);
            }

            // ReSharper disable once IteratorNeverReturns
        }
    }
}