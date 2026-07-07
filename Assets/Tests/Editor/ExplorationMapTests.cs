using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

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
    public void SeededExplorationMapAppliesReproducibleIntelBias()
    {
        ExplorationMap first = ExplorationMap.CreatePhaseOneTemplate(includeLegacyEcho: true, seed: 7);
        ExplorationMap second = ExplorationMap.CreatePhaseOneTemplate(includeLegacyEcho: true, seed: 7);

        Assert.AreEqual(first.Nodes[1].ActualRiskLevel, second.Nodes[1].ActualRiskLevel);
        Assert.AreEqual(first.Nodes[1].ActualRewardLevel, second.Nodes[1].ActualRewardLevel);
        Assert.IsTrue(first.Nodes.Any(n => n.HasRewardRiskMismatch || n.DisplayedNodeType != n.ActualNodeType));
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
        Assert.IsNotEmpty(result.IntelComparisonText);
        Assert.AreEqual(next.NodeId, controller.CurrentNodeId);
    }

    [Test]
    public void PhaseOneMapTemplateProvidesLayoutPositionsForEveryNode()
    {
        ExplorationMap map = ExplorationMap.CreatePhaseOneTemplate(includeLegacyEcho: true);

        Assert.IsTrue(map.Nodes.All(n => n.MapPosition != Vector2.zero));
        Assert.Less(map.Nodes.Min(n => n.MapPosition.x), map.Nodes.Max(n => n.MapPosition.x));
        Assert.Less(map.Nodes.Min(n => n.MapPosition.y), map.Nodes.Max(n => n.MapPosition.y));
        Assert.Less(map.FindNode("start").MapPosition.x, -300f);
        Assert.Greater(map.FindNode("end-1").MapPosition.y, 200f);
        Assert.Greater(map.FindNode("echo-1").MapPosition.x, 300f);
    }

    [Test]
    public void ExplorationMapViewRendersClickableReachableMapNodes()
    {
        ExplorationMap map = ExplorationMap.CreatePhaseOneTemplate(includeLegacyEcho: true);
        ExplorationController controller = new ExplorationController(map);
        string selectedNodeId = string.Empty;

        GameObject root = new GameObject("Map View Root", typeof(RectTransform));
        GameObject templateObject = new GameObject("Node Template", typeof(RectTransform), typeof(Image), typeof(Button));
        templateObject.transform.SetParent(root.transform, false);
        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
        labelObject.transform.SetParent(templateObject.transform, false);

        ExplorationMapView view = root.AddComponent<ExplorationMapView>();
        view.NodeButtonRoot = root.transform;
        view.NodeButtonTemplate = templateObject.GetComponent<Button>();

        view.Bind(controller, id => selectedNodeId = id);

        MapNodeIntel firstReachable = map.GetReachableNodes(map.StartNode.NodeId)[0];
        Button firstButton = view.NodeButtons.First(b => b != null && b.gameObject.activeSelf);
        RectTransform firstRect = firstButton.GetComponent<RectTransform>();
        firstButton.onClick.Invoke();

        Assert.AreEqual(firstReachable.NodeId, selectedNodeId);
        Assert.AreEqual(firstReachable.MapPosition, firstRect.anchoredPosition);
        Assert.IsTrue(firstButton.GetComponentInChildren<Text>().text.Contains(firstReachable.NodeId));

        Object.DestroyImmediate(root);
    }
}
