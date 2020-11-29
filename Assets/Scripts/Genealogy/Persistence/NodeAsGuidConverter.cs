using System;
using Newtonsoft.Json;

namespace Genealogy
{
    public class NodeAsGuidConverter : JsonConverter
    {
        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var node = (Node) value;
            serializer.Serialize(writer, new {GUID = node.Guid});
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType) => false;
    }
}