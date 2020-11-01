using System;
using System.Data;
using System.Linq;
using Newtonsoft.Json.Linq;

[Serializable]
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
        {
            throw new DataException($"Number of sub living components (${subLivingComponents.Length}) " +
                                    $"must match that of corresponding state node children (${subStateNodes.Length})");
        }

        for (var i = 0; i < subStateNodes.Length; i++)
            Load(subLivingComponents[i], subStateNodes[i]);
    }
}