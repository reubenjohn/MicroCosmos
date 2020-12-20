﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cell;
using Chemistry;
using ChemistryMicro;
using Organelles;
using Organelles.CellCauldron;
using UnityEngine;

namespace Environment
{
    public class SpontaneousLife : MonoBehaviour
    {
        public float rollDiceInterval = 1;
        public float lifeProbability = .1f;
        public float minMass = .1f;

        private IEnumerator<ChemicalBlob> blobQueue;
        private CellColony cellColony;
        private GenealogyGraphManager genealogyGraphManager;

        private Coroutine spontaneousLifeCoroutine;

        private void Start()
        {
            var cellColonyTransform = GameObject.Find("CellColony");
            cellColony = cellColonyTransform.GetComponent<CellColony>();
            genealogyGraphManager = cellColonyTransform.GetComponent<GenealogyGraphManager>();
            spontaneousLifeCoroutine = StartCoroutine(SpontaneousLifeLoop());
        }

        private void OnDestroy() => StopCoroutine(spontaneousLifeCoroutine);

        private IEnumerator SpontaneousLifeLoop()
        {
            while (true)
            {
                var blob = NextBlob();
                if (blob != null && Random.Range(0f, 1f) <= lifeProbability)
                    GiveLife(blob);
                yield return new WaitForSeconds(rollDiceInterval);
            }

            // ReSharper disable once IteratorNeverReturns
        }

        private void GiveLife(ChemicalBlob blob)
        {
            var blobMix = blob.ToMixture();
            var fatMix = new MixtureDictionary<Substance> {{Substance.Fat, blobMix.TotalMass}}
                .ToMixture();
            var reaction = new Reaction<Substance>(blobMix, fatMix);
            blob.Convert(reaction);

            var geneTree = CellGeneTranscriber.Singleton.GetTreeSampler()
                .Sample(LivingComponentRegistry.Get(Cell.Cell.ResourcePath));
            var blobTransform = blob.transform;

            var genealogyNode =
                genealogyGraphManager.RegisterAsexualCellBirth(new[] {genealogyGraphManager.RootNode});
            Debug.Log($"Blob '{blob.name}' has spontaneously come to life as '{genealogyNode.displayName}'!");
            cellColony.SpawnCell(
                geneTree,
                Cell.Cell.GetState(
                    genealogyNode.Guid,
                    cellColony.transform.InverseTransformPoint(blobTransform.position),
                    blobTransform.rotation,
                    CellCauldron.GetState(blob.ToMixture(), true)
                ),
                blob
            );
        }

        private ChemicalBlob NextBlob()
        {
            if (blobQueue == null || !blobQueue.MoveNext())
                blobQueue = transform.GetComponentsInChildren<ChemicalBlob>()
                    .Where(blob => blob != null && blob.gameObject != null && blob.TotalMass > minMass)
                    .GetEnumerator();
            if (!blobQueue.MoveNext())
                return null;
            return blobQueue.Current;
        }
    }
}