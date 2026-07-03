using System.Collections.Generic;

public class TravelerRecord
{
    public int TravelerId;
    public string DeathReason;
    public string DeathRegionId;
    public readonly List<string> DeckCardIds = new List<string>();
    public readonly List<string> RelicIds = new List<string>();
    public readonly List<string> TableAbilityIds = new List<string>();
    public readonly List<string> ConfirmedIntelIds = new List<string>();

    public TravelerRecord(int travelerId, string deathReason, string deathRegionId)
    {
        TravelerId = travelerId;
        DeathReason = deathReason;
        DeathRegionId = deathRegionId;
    }

    public static TravelerRecord FromTraveler(TravelerRun traveler)
    {
        TravelerRecord record = new TravelerRecord(traveler.TravelerId, traveler.DeathReason, traveler.DeathRegionId);
        record.DeckCardIds.AddRange(traveler.DeckCardIds);
        record.RelicIds.AddRange(traveler.RelicIds);
        record.TableAbilityIds.AddRange(traveler.ActiveTableAbilityIds);
        record.ConfirmedIntelIds.AddRange(traveler.ConfirmedIntelIds);
        return record;
    }
}
