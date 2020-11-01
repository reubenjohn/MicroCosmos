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

        public override bool CanConvert(Type objectType)
        {
            return typeof(GeneNode).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jsonObject = JObject.Load(reader);

            var geneNode = existingValue as GeneNode ?? new GeneNode();

            var resource = jsonObject["resource"];
            geneNode.resource = (string) resource;
            resource.Parent.Remove();

            geneNode.livingComponent = ((GameObject) Resources.Load(geneNode.resource)).GetComponent<ILivingComponent>();

            var gene = jsonObject["gene"];
            geneNode.gene = geneNode.livingComponent.GetGeneTranscriber().Deserialize(gene);
            gene.Parent.Remove();


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