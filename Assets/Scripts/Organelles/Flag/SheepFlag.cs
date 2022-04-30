using Genetics;
using UnityEngine;

namespace Organelles.Flag
{
    public class SheepFlag : AbstractLivingComponent<FlagGene>
    {
        public const string ResourcePath = "Organelles/SheepFlag";

        public override Transform OnInheritGene(FlagGene inheritedGene)
        {
            foreach (var rend in GetComponentsInChildren<SpriteRenderer>())
                rend.color = Color.HSVToRGB(inheritedGene.hue, 1f, 1f);
            return base.OnInheritGene(inheritedGene);
        }

        public override GeneTranscriber<FlagGene> GetGeneTranscriber() => FlagGeneTranscriber.Singleton;

        public override string GetResourcePath() => ResourcePath;
    }
}