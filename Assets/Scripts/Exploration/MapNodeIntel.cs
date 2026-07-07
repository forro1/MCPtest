using System.Collections.Generic;
using UnityEngine;

public class MapNodeIntel
{
    public string NodeId;
    public string RegionId;
    public MapNodeType DisplayedNodeType;
    public MapNodeType ActualNodeType;
    public int RiskLevel;
    public int RewardLevel;
    public int ActualRiskLevel;
    public int ActualRewardLevel;
    public int Reliability;
    public string IntelSource;
    public Vector2 MapPosition;
    public bool CanMisreadNodeType;
    public bool CanMisreadRoute;
    public bool CanMisreadRewardRisk;
    public readonly List<string> ReachableNodeIds = new List<string>();

    public MapNodeIntel(
        string nodeId,
        string regionId,
        MapNodeType displayedNodeType,
        MapNodeType actualNodeType,
        int riskLevel,
        int rewardLevel,
        int reliability,
        string intelSource)
    {
        NodeId = nodeId;
        RegionId = regionId;
        DisplayedNodeType = displayedNodeType;
        ActualNodeType = actualNodeType;
        RiskLevel = riskLevel;
        RewardLevel = rewardLevel;
        ActualRiskLevel = riskLevel;
        ActualRewardLevel = rewardLevel;
        Reliability = ClampReliability(reliability);
        IntelSource = intelSource;
    }

    public bool HasRewardRiskMismatch
    {
        get { return RiskLevel != ActualRiskLevel || RewardLevel != ActualRewardLevel; }
    }

    public bool IsUnreliable
    {
        get { return Reliability < 50 || CanMisreadNodeType || CanMisreadRoute || CanMisreadRewardRisk; }
    }

    private static int ClampReliability(int value)
    {
        if (value < 0)
        {
            return 0;
        }

        if (value > 100)
        {
            return 100;
        }

        return value;
    }
}
