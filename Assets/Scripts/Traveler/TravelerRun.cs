using System.Collections.Generic;

public class TravelerRun
{
    public int TravelerId;
    public int MaxHp;
    public int CurrentHp;
    public readonly List<string> DeckCardIds = new List<string>();
    public readonly List<string> RelicIds = new List<string>();
    public readonly List<string> ActiveTableAbilityIds = new List<string>();
    public readonly List<string> VisitedNodeIds = new List<string>();
    public readonly List<string> ConfirmedIntelIds = new List<string>();
    public readonly List<string> FoundLegacyEchoIds = new List<string>();
    public readonly List<LegacyEcho> VisibleLegacyEchoes = new List<LegacyEcho>();
    public bool IsDead;
    public string DeathReason;
    public string DeathRegionId;

    public TravelerRun(int travelerId, int maxHp, IEnumerable<string> deckCardIds)
    {
        TravelerId = travelerId;
        MaxHp = maxHp;
        CurrentHp = maxHp;
        if (deckCardIds != null)
        {
            DeckCardIds.AddRange(deckCardIds);
        }
    }

    public void IncreaseMaxHp(int amount, bool healForIncrease)
    {
        if (amount <= 0)
        {
            return;
        }

        MaxHp += amount;
        if (healForIncrease)
        {
            CurrentHp += amount;
        }
    }

    public void MarkDead(string reason, string regionId)
    {
        IsDead = true;
        CurrentHp = 0;
        DeathReason = reason;
        DeathRegionId = regionId;
    }
}
