using System.Collections;
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
        public float rollDiceInterval = .1f;
        public float coolDownInterval = 5;
        public float lifeProbability = .1f;
        public float minMassFactor = 2;

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
            yield return null;
            while (true)
            {
                var blobs = transform.GetComponentsInChildren<ChemicalBlob>()
                    .Where(blob =>
                        blob != null &&
                        blob[Substance.Fat] > Cell.Cell.MinMass * minMassFactor &&
                        blob.gameObject != null).ToArray();
                if (!blobs.Any())
                    yield return new WaitForSeconds(coolDownInterval);
                foreach (var blob in blobs)
                {
                    if (blob == null)
                        continue;
                    if (Random.Range(0f, 1f) <= lifeProbability)
                        GiveLife(blob);
                    yield return new WaitForSeconds(rollDiceInterval);
                }
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