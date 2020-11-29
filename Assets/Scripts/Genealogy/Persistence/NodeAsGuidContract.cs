using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Genealogy.Persistence
{
    public class NodeAsGuidContract : DefaultContractResolver
    {
        private readonly NodeAsGuidConverter converter;

        public NodeAsGuidContract()
        {
            converter = new NodeAsGuidConverter();
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (member.GetCustomAttribute<NodeAsGuidAttribute>() != null)
            {
                property.Converter = converter;
                property.MemberConverter = converter;
            }

            return property;
        }
    }
}