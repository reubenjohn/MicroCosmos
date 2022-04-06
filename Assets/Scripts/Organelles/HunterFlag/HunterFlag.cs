using Genetics;
using UnityEngine;

namespace Organelles.HunterFlag
{
    public class HunterFlag : AbstractLivingComponent<HunterFlagGene>
    {
        public const string ResourcePath = "Organelles/HunterFlag";

        public override Transform OnInheritGene(HunterFlagGene inheritedGene)
        {
            foreach (var rend in GetComponentsInChildren<SpriteRenderer>())
                rend.color = Color.HSVToRGB(inheritedGene.hue, 1f, 1f);
            return base.OnInheritGene(inheritedGene);
        }

        public override GeneTranscriber<HunterFlagGene> GetGeneTranscriber() => HunterFlagGeneTranscriber.Singleton;

        public override string GetResourcePath() => ResourcePath;
    }
}