using System.Collections.Generic;
using Genetics;
using Newtonsoft.Json;
using NUnit.Framework;
using Organelles.SimpleContainment;
using UnityEngine;

namespace Tests.EditMode.Organelles.SimpleContainment
{
    public class SubOrganelleCountsSimpleMutatorTest
    {
        [Test]
        public void NewTestScriptSimplePasses()
        {
            var mutator = new SubOrganelleCountsSimpleMutator(new Dictionary<string, GeneMutator<float>>
            {
                {"a", x => x * .9f},
                {"b", x => .6f},
                {"c", x => x},
                {"d", x => x * .5f}
            });
            Random.InitState(0);
            var mutated = mutator.Mutate(new SubOrganelleCounts
            {
                {"a", .5f},
                {"b", .5f},
                {"c", .5f},
                {"e", .25f}
            });
            Assert.AreEqual(@"{""a"":0.45,""b"":0.6,""c"":0.5,""d"":0.312735647}",
                JsonConvert.SerializeObject(mutated));
        }
    }
}