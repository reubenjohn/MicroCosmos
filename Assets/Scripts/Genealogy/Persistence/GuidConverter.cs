using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Genealogy.Persistence
{
    public class GuidConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            writer.WriteValue(value.ToString());

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) =>
            Guid.Parse((string) JToken.Load(reader));

        public override bool CanConvert(Type objectType) => objectType == typeof(Guid);
    }
}