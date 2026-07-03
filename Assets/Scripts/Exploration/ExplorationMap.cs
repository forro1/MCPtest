using System.Collections.Generic;

public class ExplorationMap
{
    public readonly List<MapNodeIntel> Nodes = new List<MapNodeIntel>();

    public MapNodeIntel StartNode
    {
        get { return Nodes.Count == 0 ? null : Nodes[0]; }
    }

    public static ExplorationMap CreatePhaseOneTemplate(bool includeLegacyEcho)
    {
        return CreatePhaseOneTemplate(includeLegacyEcho, 0);
    }

    public static ExplorationMap CreatePhaseOneTemplate(bool includeLegacyEcho, int seed)
    {
        ExplorationMap map = new ExplorationMap();
        MapNodeIntel start = new MapNodeIntel("start", "village-road", MapNodeType.Start, MapNodeType.Start, 0, 0, 100, "Village");
        MapNodeIntel battle = new MapNodeIntel("battle-1", "mist-woods", MapNodeType.Battle, MapNodeType.Battle, 2, 2, 80, "Old route");
        MapNodeIntel eventNode = new MapNodeIntel("event-1", "mist-woods", MapNodeType.Event, MapNodeType.Event, 1, 2, 65, "Traveler rumor");
        MapNodeIntel rest = new MapNodeIntel("rest-1", "stone-camp", MapNodeType.Rest, MapNodeType.Rest, 0, 1, 75, "Camp notes");
        MapNodeIntel echo = new MapNodeIntel("echo-1", "mist-woods", MapNodeType.LegacyEcho, includeLegacyEcho ? MapNodeType.LegacyEcho : MapNodeType.Event, 1, 3, 55, "Legacy clue");
        MapNodeIntel end = new MapNodeIntel("end-1", "old-gate", MapNodeType.RegionEnd, MapNodeType.RegionEnd, 3, 3, 70, "Village estimate");

        eventNode.CanMisreadRewardRisk = true;
        echo.CanMisreadNodeType = !includeLegacyEcho;
        ApplySeededBias(new List<MapNodeIntel> { battle, eventNode, rest, echo, end }, seed);

        start.ReachableNodeIds.Add(battle.NodeId);
        start.ReachableNodeIds.Add(eventNode.NodeId);
        battle.ReachableNodeIds.Add(rest.NodeId);
        eventNode.ReachableNodeIds.Add(echo.NodeId);
        rest.ReachableNodeIds.Add(end.NodeId);
        echo.ReachableNodeIds.Add(end.NodeId);

        map.Nodes.Add(start);
        map.Nodes.Add(battle);
        map.Nodes.Add(eventNode);
        map.Nodes.Add(rest);
        map.Nodes.Add(echo);
        map.Nodes.Add(end);
        return map;
    }

    private static void ApplySeededBias(List<MapNodeIntel> nodes, int seed)
    {
        System.Random random = new System.Random(seed);
        for (int i = 0; i < nodes.Count; i++)
        {
            MapNodeIntel node = nodes[i];
            if (node.Reliability >= 90)
            {
                continue;
            }

            int riskDelta = random.Next(-1, 2);
            int rewardDelta = random.Next(-1, 2);
            node.ActualRiskLevel = ClampLevel(node.RiskLevel + riskDelta);
            node.ActualRewardLevel = ClampLevel(node.RewardLevel + rewardDelta);
            if (riskDelta != 0 || rewardDelta != 0)
            {
                node.CanMisreadRewardRisk = true;
            }
        }

        // Keep one deterministic visible mismatch in the template so the UI has a Phase 1 signal to explain.
        if (nodes.Count > 1 && !nodes[1].HasRewardRiskMismatch)
        {
            nodes[1].ActualRiskLevel = ClampLevel(nodes[1].RiskLevel + 1);
            nodes[1].CanMisreadRewardRisk = true;
        }
    }

    private static int ClampLevel(int value)
    {
        if (value < 0)
        {
            return 0;
        }

        if (value > 5)
        {
            return 5;
        }

        return value;
    }

    public MapNodeIntel FindNode(string nodeId)
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            if (Nodes[i].NodeId == nodeId)
            {
                return Nodes[i];
            }
        }

        return null;
    }

    public List<MapNodeIntel> GetReachableNodes(string nodeId)
    {
        List<MapNodeIntel> reachable = new List<MapNodeIntel>();
        MapNodeIntel node = FindNode(nodeId);
        if (node == null)
        {
            return reachable;
        }

        for (int i = 0; i < node.ReachableNodeIds.Count; i++)
        {
            MapNodeIntel next = FindNode(node.ReachableNodeIds[i]);
            if (next != null)
            {
                reachable.Add(next);
            }
        }

        return reachable;
    }
}
