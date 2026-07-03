public class LegacyEcho
{
    public string EchoId;
    public int SourceTravelerId;
    public string RegionHint;
    public string Cause;
    public string RewardPayload;
    public string ImmediateClaimEffect = "current_hp_and_max_hp";
    public string ResearchEffect = "training_level";
    public bool IsRecovered;

    public LegacyEcho(string echoId, int sourceTravelerId, string regionHint, string cause, string rewardPayload)
    {
        EchoId = echoId;
        SourceTravelerId = sourceTravelerId;
        RegionHint = regionHint;
        Cause = cause;
        RewardPayload = rewardPayload;
    }
}
