using Newtonsoft.Json;

namespace Genealogy.Persistence
{
    public static class ScrollReader
    {
        public static void Load(JsonReader sw, GenealogyGraph graph)
        {
            var jsonSerializer = new JsonSerializer()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = new GenealogyScrollContractResolver(graph)
            };
            jsonSerializer.Deserialize<GenealogyScroll>(sw);
        }
    }
}