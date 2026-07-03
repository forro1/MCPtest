using System;

public class ExplorationController
{
    public ExplorationMap Map { get; private set; }
    public string CurrentNodeId { get; private set; }

    public ExplorationController(ExplorationMap map)
    {
        Map = map;
        CurrentNodeId = map != null && map.StartNode != null ? map.StartNode.NodeId : string.Empty;
    }

    public ExplorationNodeResult EnterNode(string nodeId)
    {
        MapNodeIntel target = Map == null ? null : Map.FindNode(nodeId);
        if (target == null)
        {
            throw new ArgumentException("Unknown exploration node: " + nodeId);
        }

        bool reachable = false;
        System.Collections.Generic.List<MapNodeIntel> reachableNodes = Map.GetReachableNodes(CurrentNodeId);
        for (int i = 0; i < reachableNodes.Count; i++)
        {
            if (reachableNodes[i].NodeId == nodeId)
            {
                reachable = true;
                break;
            }
        }

        if (!reachable)
        {
            throw new InvalidOperationException("Node is not reachable: " + nodeId);
        }

        CurrentNodeId = nodeId;
        return new ExplorationNodeResult(target);
    }
}
