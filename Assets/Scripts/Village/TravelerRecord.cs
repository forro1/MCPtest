public class TravelerRecord
{
    public int TravelerId;
    public string DeathReason;
    public string DeathRegionId;

    public TravelerRecord(int travelerId, string deathReason, string deathRegionId)
    {
        TravelerId = travelerId;
        DeathReason = deathReason;
        DeathRegionId = deathRegionId;
    }
}
