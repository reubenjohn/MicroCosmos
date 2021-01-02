﻿using System.Collections;
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
        private static readonly Mixture<Substance> UnitFatMix =
            new MixtureDictionary<Substance> {{Substance.Fat, 1f}}.ToMixture();

        public float minMassFactor = 2;
        public int maxCellCount = 50;
        private CellColony cellColony;

        private Environment environment;
        private GenealogyGraphManager genealogyGraphManager;
        private float lifeProbability = .5f;

        private float rollDiceInterval = .1f;

        private void Start()
        {
            environment = GetComponent<Environment>();
            cellColony = GetComponentInChildren<CellColony>();
            genealogyGraphManager = GetComponentInChildren<GenealogyGraphManager>();
            StartCoroutine(SpontaneousLifeLoop());
        }

        private void OnDestroy() => StopAllCoroutines();

        private IEnumerator SpontaneousLifeLoop()
        {
            yield return null;

            while (true)
            {
                var blobs = GetComponentsInChildren<ChemicalBlob>()
                    .Where(blob => blob.TotalMass > Cell.Cell.MinMass * minMassFactor);
                var foundBlobs = false;

                foreach (var blob in blobs)
                {
                    while (environment.CellCount >= maxCellCount)
                    {
                        rollDiceInterval = Mathf.Min(rollDiceInterval * 1.1f, 1f);
                        lifeProbability = lifeProbability * .9f;
                        Grapher.Log(rollDiceInterval, "SpontaneousLife.rollDiceInterval");
                        Grapher.Log(lifeProbability, "SpontaneousLife.lifeProbability");
                        yield return new WaitForSeconds(rollDiceInterval);
                    }

                    rollDiceInterval = Mathf.Max(rollDiceInterval * .9f, .25f);
                    Grapher.Log(rollDiceInterval, "SpontaneousLife.rollDiceInterval");
                    Grapher.Log(lifeProbability, "SpontaneousLife.lifeProbability");

                    if (blob == null)
                        continue;
                    foundBlobs = true;
                    if (Random.Range(0f, 1f) <= lifeProbability)
                        GiveLife(blob);
                    yield return new WaitForSeconds(rollDiceInterval);
                }

                if (environment.CellCount < maxCellCount)
                    lifeProbability += (1f - lifeProbability) * .1f;

                if (!foundBlobs)
                    rollDiceInterval = Mathf.Min(rollDiceInterval * 1.1f, 1f);

                yield return new WaitForSeconds(rollDiceInterval);
            }

            // ReSharper disable once IteratorNeverReturns
        }

        private void GiveLife(ChemicalBlob blob)
        {
            var blobMix = blob.ToMixture();
            var reaction = new Reaction<Substance>(blobMix, UnitFatMix * blobMix.TotalMass);
            blob.Convert(reaction);

            var geneTree = CellGeneTranscriber.Singleton.GetTreeSampler()
                .Sample(LivingComponentRegistry.Get(Cell.Cell.ResourcePath));
            var blobTransform = blob.transform;

            var genealogyNode =
                genealogyGraphManager.RegisterAsexualCellBirth(new[] {genealogyGraphManager.RootNode});
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
    }
}