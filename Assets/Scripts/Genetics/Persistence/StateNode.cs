using System.Data;
using System.Linq;
using Genetics;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Persistence
{
    [NoReorder]
    public class StateNode
    {
        public JObject state;
        public StateNode[] children;

        public static StateNode Save(ILivingComponent livingComponent) =>
            new StateNode
            {
                state = livingComponent.GetState(),
                children = livingComponent.GetSubLivingComponents()
                    .Select(Save)
                    .ToArray()
            };

        public static void Load(ILivingComponent livingComponent, StateNode stateNode)
        {
            livingComponent.SetState(stateNode.state);

            var subLivingComponents = livingComponent.GetSubLivingComponents();
            var subStateNodes = stateNode.children;
            if (subLivingComponents.Length != subStateNodes.Length)
                throw new DataException(
                    $"Living component '{livingComponent.GetNodeName()}' has " +
                    $"{subLivingComponents.Length} sub living components " +
                    $"and thus can't be loaded with {subStateNodes.Length} state nodes:\n" +
                    $"{JsonConvert.SerializeObject(subStateNodes, Formatting.Indented)}");

            for (var i = 0; i < subStateNodes.Length; i++)
                Load(subLivingComponents[i], subStateNodes[i]);
        }

        public static StateNode Empty(GeneNode geneNode) =>
            new StateNode
            {
                state = new JObject(),
                children = geneNode.children
                    .Select(Empty)
                    .ToArray()
            };
    }
}