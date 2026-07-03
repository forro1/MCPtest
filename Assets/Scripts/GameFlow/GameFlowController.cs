public class GameFlowController
{
    public VillageState Village { get; private set; }
    public TravelerRun CurrentTraveler { get; private set; }
    public ExplorationController Exploration { get; private set; }
    public GamePhase Phase { get; private set; }

    public GameFlowController(VillageState village)
    {
        Village = village ?? new VillageState();
        Phase = GamePhase.Village;
    }

    public TravelerRun StartNewTraveler()
    {
        CurrentTraveler = TravelerFactory.CreateTraveler(Village);
        Exploration = new ExplorationController(ExplorationMap.CreatePhaseOneTemplate(CurrentTraveler.VisibleLegacyEchoes.Count > 0));
        Phase = GamePhase.Exploring;
        return CurrentTraveler;
    }

    public ExplorationNodeResult EnterNode(string nodeId)
    {
        ExplorationNodeResult result = Exploration.EnterNode(nodeId);
        if (CurrentTraveler != null)
        {
            CurrentTraveler.VisitedNodeIds.Add(nodeId);
        }

        RecordConfirmedIntel(result.Node);

        if (result.ResultType == MapNodeType.Battle)
        {
            Phase = GamePhase.Battle;
        }
        else if (result.ResultType == MapNodeType.LegacyEcho)
        {
            Phase = GamePhase.LegacyEcho;
        }

        return result;
    }

    public void RecordConfirmedIntel(MapNodeIntel node)
    {
        if (node == null)
        {
            return;
        }

        if (CurrentTraveler != null && !CurrentTraveler.ConfirmedIntelIds.Contains(node.NodeId))
        {
            CurrentTraveler.ConfirmedIntelIds.Add(node.NodeId);
        }

        Village.AddMapIntel(node);
    }

    public LegacyEcho MarkTravelerDead(string reason, string regionId)
    {
        if (CurrentTraveler == null)
        {
            StartNewTraveler();
        }

        CurrentTraveler.MarkDead(reason, regionId);
        Village.AddTravelerRecord(new TravelerRecord(CurrentTraveler.TravelerId, reason, regionId));
        LegacyEcho echo = LegacyEchoFactory.CreateFromDeath(CurrentTraveler);
        Village.AddLegacyEcho(echo);
        Phase = GamePhase.RunSummary;
        return echo;
    }

    public void ApplyBattleResult(BattleRunResult result, string regionId)
    {
        if (result == null)
        {
            return;
        }

        if (CurrentTraveler != null)
        {
            CurrentTraveler.CurrentHp = result.RemainingHp;
        }

        if (result.IsDefeat)
        {
            MarkTravelerDead(result.DeathReason, regionId);
            return;
        }

        if (result.IsVictory)
        {
            Phase = GamePhase.Exploring;
        }
    }

    public bool ResolveVisibleEcho(bool immediate)
    {
        if (CurrentTraveler == null)
        {
            return false;
        }

        LegacyEcho echo = null;
        for (int i = 0; i < CurrentTraveler.VisibleLegacyEchoes.Count; i++)
        {
            if (!CurrentTraveler.VisibleLegacyEchoes[i].IsRecovered)
            {
                echo = CurrentTraveler.VisibleLegacyEchoes[i];
                break;
            }
        }

        if (echo == null)
        {
            return false;
        }

        bool resolved = immediate
            ? LegacyEchoResolver.ResolveImmediate(echo, CurrentTraveler, Village)
            : LegacyEchoResolver.ResolveResearch(echo, CurrentTraveler, Village);
        if (resolved)
        {
            Phase = GamePhase.Exploring;
        }

        return resolved;
    }
}
