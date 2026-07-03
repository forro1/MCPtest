using System.Collections.Generic;

public class VillageState
{
    private int nextTravelerId = 1;

    public int ActPhase = 1;
    public int TrainingLevel;
    public readonly List<string> UnlockedCardIds = new List<string>();
    public readonly List<MapNodeIntel> MapIntelRecords = new List<MapNodeIntel>();
    public readonly List<TravelerRecord> TravelerRecords = new List<TravelerRecord>();
    public readonly List<LegacyEcho> LegacyEchoes = new List<LegacyEcho>();
    public readonly TableProgress TableProgress = new TableProgress();

    public void AddTravelerRecord(TravelerRecord record)
    {
        if (record != null)
        {
            TravelerRecords.Add(record);
        }
    }

    public void AddLegacyEcho(LegacyEcho echo)
    {
        if (echo != null)
        {
            LegacyEchoes.Add(echo);
        }
    }

    public int NextTravelerId()
    {
        int travelerId = nextTravelerId;
        nextTravelerId++;
        return travelerId;
    }

    public void AddMapIntel(MapNodeIntel node)
    {
        if (node == null)
        {
            return;
        }

        for (int i = 0; i < MapIntelRecords.Count; i++)
        {
            if (MapIntelRecords[i] != null && MapIntelRecords[i].NodeId == node.NodeId)
            {
                MapIntelRecords[i] = node;
                return;
            }
        }

        MapIntelRecords.Add(node);
    }
}
