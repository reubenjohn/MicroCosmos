using System.Collections;
using System.IO;
using System.Linq;
using Chemistry;
using ChemistryMicro;
using Newtonsoft.Json;
using Organelles.CellCauldron;
using Persistence;
using UnityEngine;

namespace Environment
{
    public class SpontaneousSheep : MonoBehaviour
    {
        private static readonly Mixture<Substance> UnitFatMix =
            new MixtureDictionary<Substance> { { Substance.Fat, 1f } }.ToMixture();

        public float minMassFactor = 2;
        public int maxCellCount = 50;
        private CellColony cellColony;

        private Environment environment;
        private GenealogyGraphManager genealogyGraphManager;
        private GeneNode sheepGenes;

        private float lifeProbability = .5f;
        private float rollDiceInterval = .1f;

        private void Start()
        {
            environment = GetComponent<Environment>();
            cellColony = GetComponentInChildren<CellColony>();
            genealogyGraphManager = GetComponentInChildren<GenealogyGraphManager>();

            var textAsset = Resources.Load<TextAsset>("SheepGenes");
            using (JsonReader reader = new JsonTextReader(new StringReader(textAsset.text)))
            {
                sheepGenes = cellColony.GetSerializer().Deserialize<GeneNode>(reader);
            }

            StartCoroutine(SpontaneousLifeLoop());
            StartCoroutine(GrapherLoop());
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
                        rollDiceInterval.CreepTo(1f, .1f);
                        yield return new WaitForSeconds(rollDiceInterval);
                    }

                    rollDiceInterval.CreepTo(.05f, .1f);
                    lifeProbability =
                        Mathf.Clamp01((maxCellCount - environment.CellCount) / (float)environment.ChemicalBlobCount);

                    if (blob == null)
                        continue;
                    foundBlobs = true;
                    if (Random.Range(0f, 1f) <= lifeProbability)
                    {
                        GiveLife(blob);
                        yield return new WaitForSeconds(rollDiceInterval);
                    }
                }

                if (!foundBlobs)
                    rollDiceInterval.CreepTo(1f, .1f);

                yield return new WaitForSeconds(rollDiceInterval);
            }

            // ReSharper disable once IteratorNeverReturns
        }

        private void GiveLife(ChemicalBlob blob)
        {
            var blobMix = blob.ToMixture();
            var reaction = new Reaction<Substance>(blobMix, UnitFatMix * blobMix.TotalMass);
            blob.Convert(reaction);

            var blobTransform = blob.transform;

            var genealogyNode =
                genealogyGraphManager.RegisterAsexualCellBirth(new[] { genealogyGraphManager.RootNode });
            cellColony.SpawnCell(
                sheepGenes,
                Cell.Cell.GetState(
                    genealogyNode.Guid,
                    cellColony.transform.InverseTransformPoint(blobTransform.position),
                    blobTransform.rotation,
                    CellCauldron.GetState(blob.ToMixture(), true)
                ),
                blob
            );
        }

        private IEnumerator GrapherLoop()
        {
            yield return null;

            while (true)
            {
                Grapher.Log(rollDiceInterval, "SpontaneousLife.rollDiceInterval");
                Grapher.Log(lifeProbability, "SpontaneousLife.lifeProbability");
                yield return new WaitForSeconds(.5f);
            }

            // ReSharper disable once IteratorNeverReturns
        }
    }
}