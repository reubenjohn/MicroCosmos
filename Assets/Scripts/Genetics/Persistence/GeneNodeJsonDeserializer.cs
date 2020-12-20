using System;
using Genetics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Persistence
{
    public class GeneNodeJsonDeserializer : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType) => typeof(GeneNode).IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jsonObject = JObject.Load(reader);

            var resourceToken = jsonObject["resource"];
            var resource = (string) resourceToken;
            resourceToken.Parent.Remove();

            var gameObject = (GameObject) Resources.Load(resource);
            if (gameObject == null)
                throw new ArgumentNullException($"Resource {resource} not found");

            var livingComponent = gameObject.GetComponent<ILivingComponent>();

            var geneToken = jsonObject["gene"];
            var gene = livingComponent.GetGeneTranscriber().Deserialize(geneToken);
            geneToken.Parent.Remove();

            var geneNode = new GeneNode(livingComponent, gene, new GeneNode[0]);

            using (var subReader = jsonObject.CreateReader())
            {
                serializer.Populate(subReader, geneNode);
            }

            return geneNode;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            throw new NotImplementedException();
    }
}