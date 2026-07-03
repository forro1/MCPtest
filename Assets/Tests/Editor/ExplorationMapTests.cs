using System.Linq;
using NUnit.Framework;

public class ExplorationMapTests
{
    [Test]
    public void MapNodeIntelCanRepresentUnreliableDisplayedInformation()
    {
        MapNodeIntel node = new MapNodeIntel(
            "n-1",
            "mist-woods",
            MapNodeType.Event,
            MapNodeType.Battle,
            3,
            2,
            35,
            "Traveler rumor");
        node.CanMisreadNodeType = true;
        node.CanMisreadRewardRisk = true;

        Assert.AreEqual(MapNodeType.Event, node.DisplayedNodeType);
        Assert.AreEqual(MapNodeType.Battle, node.ActualNodeType);
        Assert.AreEqual(35, node.Reliability);
        Assert.IsTrue(node.IsUnreliable);
    }

    [Test]
    public void ExplorationMapCreatesSmallReachableTemplate()
    {
        ExplorationMap map = ExplorationMap.CreatePhaseOneTemplate(includeLegacyEcho: true);

        Assert.GreaterOrEqual(map.Nodes.Count, 5);
        Assert.LessOrEqual(map.Nodes.Count, 8);
        Assert.IsNotNull(map.StartNode);
        Assert.GreaterOrEqual(map.GetReachableNodes(map.StartNode.NodeId).Count, 1);
        Assert.GreaterOrEqual(map.Nodes.Select(n => n.ActualNodeType).Distinct().Count(), 3);
    }

    [Test]
    public void ExplorationControllerReturnsNodeResultWhenEnteringReachableNode()
    {
        ExplorationMap map = ExplorationMap.CreatePhaseOneTemplate(includeLegacyEcho: true);
        ExplorationController controller = new ExplorationController(map);
        MapNodeIntel next = map.GetReachableNodes(map.StartNode.NodeId)[0];

        ExplorationNodeResult result = controller.EnterNode(next.NodeId);

        Assert.AreEqual(next.NodeId, result.Node.NodeId);
        Assert.AreEqual(next.ActualNodeType, result.ResultType);
        Assert.AreEqual(next.NodeId, controller.CurrentNodeId);
    }
}
