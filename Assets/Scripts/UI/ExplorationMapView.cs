using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplorationMapView : MonoBehaviour
{
    public Text CurrentNodeText;
    public Text ReachableIntelText;
    public Transform NodeButtonRoot;
    public Button NodeButtonTemplate;
    public List<Button> NodeButtons = new List<Button>();

    private ExplorationMap map;
    private string currentNodeId;
    private Action<string> nodeSelected;

    public void Bind(ExplorationController controller, Action<string> onNodeSelected)
    {
        if (controller == null)
        {
            Bind(null, string.Empty, onNodeSelected);
            return;
        }

        Bind(controller.Map, controller.CurrentNodeId, onNodeSelected);
    }

    public void Bind(ExplorationMap explorationMap, string nodeId, Action<string> onNodeSelected)
    {
        map = explorationMap;
        currentNodeId = nodeId;
        nodeSelected = onNodeSelected;
        Render(map, currentNodeId);
    }

    public void Render(ExplorationController controller)
    {
        if (controller == null)
        {
            Render(null, string.Empty);
            return;
        }

        Render(controller.Map, controller.CurrentNodeId);
    }

    public void Render(ExplorationMap explorationMap, string nodeId)
    {
        map = explorationMap;
        currentNodeId = nodeId;

        List<MapNodeIntel> reachable = map == null ? new List<MapNodeIntel>() : map.GetReachableNodes(currentNodeId);
        SetText(CurrentNodeText, "当前位置：" + EmptyToUnknown(currentNodeId));
        SetText(ReachableIntelText, BuildReachableIntelText(reachable));
        RenderNodeButtons(reachable);
    }

    private void RenderNodeButtons(List<MapNodeIntel> reachable)
    {
        EnsureButtonCount(reachable.Count);

        for (int i = 0; i < NodeButtons.Count; i++)
        {
            Button button = NodeButtons[i];
            if (button == null)
            {
                continue;
            }

            bool active = i < reachable.Count;
            button.gameObject.SetActive(active);
            button.onClick.RemoveAllListeners();

            if (!active)
            {
                continue;
            }

            MapNodeIntel node = reachable[i];
            string nodeId = node.NodeId;
            Text label = button.GetComponentInChildren<Text>();
            SetText(label, BuildButtonLabel(node));
            button.onClick.AddListener(delegate
            {
                if (nodeSelected != null)
                {
                    nodeSelected(nodeId);
                }
            });
        }
    }

    private void EnsureButtonCount(int count)
    {
        if (NodeButtonTemplate == null)
        {
            return;
        }

        Transform root = NodeButtonRoot == null ? NodeButtonTemplate.transform.parent : NodeButtonRoot;
        if (!NodeButtons.Contains(NodeButtonTemplate))
        {
            NodeButtons.Add(NodeButtonTemplate);
        }

        while (NodeButtons.Count < count)
        {
            Button button = Instantiate(NodeButtonTemplate, root);
            button.name = "Node Button " + NodeButtons.Count;
            NodeButtons.Add(button);
        }
    }

    private static string BuildReachableIntelText(List<MapNodeIntel> reachable)
    {
        if (reachable == null || reachable.Count == 0)
        {
            return "可达节点：无";
        }

        List<string> lines = new List<string>();
        lines.Add("可达节点情报：");
        for (int i = 0; i < reachable.Count; i++)
        {
            lines.Add(BuildIntelLine(reachable[i]));
        }

        return string.Join("\n", lines.ToArray());
    }

    private static string BuildIntelLine(MapNodeIntel node)
    {
        if (node == null)
        {
            return "- 未知节点";
        }

        string unreliable = node.IsUnreliable ? "（可疑）" : string.Empty;
        return "- " + EmptyToUnknown(node.NodeId)
            + "  类型：" + node.DisplayedNodeType + unreliable
            + "  风险：" + node.RiskLevel
            + "  收益：" + node.RewardLevel
            + "  可信度：" + node.Reliability + "%"
            + "  来源：" + EmptyToUnknown(node.IntelSource);
    }

    private static string BuildButtonLabel(MapNodeIntel node)
    {
        if (node == null)
        {
            return "进入未知节点";
        }

        return "进入 " + EmptyToUnknown(node.NodeId) + " / " + node.DisplayedNodeType;
    }

    private static string EmptyToUnknown(string value)
    {
        return string.IsNullOrEmpty(value) ? "未知" : value;
    }

    private static void SetText(Text text, string value)
    {
        if (text != null)
        {
            text.text = value;
        }
    }
}
