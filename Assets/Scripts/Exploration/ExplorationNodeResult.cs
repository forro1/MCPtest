public class ExplorationNodeResult
{
    public MapNodeIntel Node;
    public MapNodeType ResultType;
    public string IntelComparisonText;

    public ExplorationNodeResult(MapNodeIntel node)
    {
        Node = node;
        ResultType = node == null ? MapNodeType.Event : node.ActualNodeType;
        IntelComparisonText = BuildIntelComparisonText(node);
    }

    private static string BuildIntelComparisonText(MapNodeIntel node)
    {
        if (node == null)
        {
            return "情报缺失，实际结果未知。";
        }

        return "情报预测：" + node.DisplayedNodeType
            + " 风险" + node.RiskLevel
            + " 收益" + node.RewardLevel
            + "；实际：" + node.ActualNodeType
            + " 风险" + node.ActualRiskLevel
            + " 收益" + node.ActualRewardLevel + "。";
    }
}
