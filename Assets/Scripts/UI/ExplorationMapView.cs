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
            RectTransform rect = button.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = node.MapPosition;
                rect.sizeDelta = new Vector2(132f, 56f);
            }

            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = GetNodeColor(node.DisplayedNodeType);
            }

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

        return GetNodeTypeLabel(node.DisplayedNodeType) + "\n" + EmptyToUnknown(node.NodeId);
    }

    private static string GetNodeTypeLabel(MapNodeType type)
    {
        switch (type)
        {
            case MapNodeType.Battle:
                return "战斗";
            case MapNodeType.Event:
                return "事件";
            case MapNodeType.Rest:
                return "营地";
            case MapNodeType.LegacyEcho:
                return "回声";
            case MapNodeType.RegionEnd:
                return "终点";
            case MapNodeType.Start:
                return "起点";
            default:
                return "节点";
        }
    }

    private static Color GetNodeColor(MapNodeType type)
    {
        switch (type)
        {
            case MapNodeType.Battle:
                return new Color(0.62f, 0.22f, 0.24f);
            case MapNodeType.Event:
                return new Color(0.38f, 0.42f, 0.62f);
            case MapNodeType.Rest:
                return new Color(0.24f, 0.50f, 0.38f);
            case MapNodeType.LegacyEcho:
                return new Color(0.48f, 0.32f, 0.66f);
            case MapNodeType.RegionEnd:
                return new Color(0.70f, 0.56f, 0.26f);
            default:
                return new Color(0.32f, 0.42f, 0.54f);
        }
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
