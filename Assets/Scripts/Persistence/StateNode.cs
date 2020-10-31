using System.Linq;
using Newtonsoft.Json.Linq;

public class StateNode
{
    public JObject state;
    public StateNode[] children;


    public static StateNode Save(ILivingComponent livingComponent)
    {
        return new StateNode()
        {
            state = livingComponent.GetState(),
            children = livingComponent.GetSubLivingComponents()
                .Select(subLivingComponent => StateNode.Save(subLivingComponent))
                .ToArray()
        };
    }

    public static void Load(ILivingComponent livingComponent, StateNode stateNode)
    {
        livingComponent.SetState(stateNode.state);
        Enumerable.Zip(livingComponent.GetSubLivingComponents(), stateNode.children, (subLivingComponent, subStateNode) =>
        {
            StateNode.Load(subLivingComponent, subStateNode);
            return true;
        });
    }
}
