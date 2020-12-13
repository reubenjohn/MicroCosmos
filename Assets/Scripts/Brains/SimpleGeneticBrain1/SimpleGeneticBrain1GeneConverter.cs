using System;
using Newtonsoft.Json;

namespace Brains.SimpleGeneticBrain1
{
    public class SimpleGeneticBrain1GeneConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            throw new NotImplementedException();

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var gene = new SimpleGeneticBrain1Gene(SimpleGeneticBrain1GeneTranscriber.Repairer);
            serializer.Populate(reader, gene);
            return gene;
        }

        public override bool CanConvert(Type objectType) =>
            typeof(SimpleGeneticBrain1Gene).IsAssignableFrom(objectType);
    }
}