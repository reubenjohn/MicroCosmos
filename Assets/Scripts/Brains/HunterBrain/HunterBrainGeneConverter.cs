using System;
using Newtonsoft.Json;

namespace Brains.HunterBrain
{
    public class HunterBrainGeneConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            throw new NotImplementedException();

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var gene = new HunterBrainGene(HunterBrainGeneTranscriber.Repairer);
            serializer.Populate(reader, gene);
            return gene;
        }

        public override bool CanConvert(Type objectType) => typeof(HunterBrainGene).IsAssignableFrom(objectType);
    }
}