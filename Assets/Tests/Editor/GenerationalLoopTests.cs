using System.Collections.Generic;
using NUnit.Framework;

public class GenerationalLoopTests
{
    [Test]
    public void EmptyVillageStartsWithDefaultLongTermState()
    {
        VillageState village = new VillageState();

        Assert.AreEqual(0, village.TrainingLevel);
        Assert.AreEqual(0, village.TravelerRecords.Count);
        Assert.AreEqual(0, village.LegacyEchoes.Count);
        Assert.IsNotNull(village.TableProgress);
    }

    [Test]
    public void VillageTracksTravelersAndEchoes()
    {
        VillageState village = new VillageState();
        TravelerRecord record = new TravelerRecord(1, "Fell in battle", "mist-woods");
        LegacyEcho echo = new LegacyEcho("echo-1", 1, "mist-woods", "Fell in battle", "Recovered strike");

        village.AddTravelerRecord(record);
        village.AddLegacyEcho(echo);

        Assert.AreEqual(1, village.TravelerRecords.Count);
        Assert.AreEqual(record, village.TravelerRecords[0]);
        Assert.AreEqual(1, village.LegacyEchoes.Count);
        Assert.AreEqual(echo, village.LegacyEchoes[0]);
    }

    [Test]
    public void TravelerFactoryUsesTrainingWithoutChangingVillageHistory()
    {
        VillageState village = new VillageState();
        village.TrainingLevel = 2;
        village.AddLegacyEcho(new LegacyEcho("echo-1", 1, "mist-woods", "Fell in battle", "Recovered strike"));

        TravelerRun traveler = TravelerFactory.CreateTraveler(village);

        Assert.AreEqual(1, traveler.TravelerId);
        Assert.AreEqual(54, traveler.MaxHp);
        Assert.AreEqual(traveler.MaxHp, traveler.CurrentHp);
        Assert.GreaterOrEqual(traveler.DeckCardIds.Count, 1);
        Assert.AreEqual(0, village.TravelerRecords.Count);
        Assert.AreEqual(1, village.LegacyEchoes.Count);
    }

    [Test]
    public void TravelerFactoryAllocatesUniqueIdsBeforeDeathRecords()
    {
        VillageState village = new VillageState();

        TravelerRun first = TravelerFactory.CreateTraveler(village);
        TravelerRun second = TravelerFactory.CreateTraveler(village);

        Assert.AreEqual(1, first.TravelerId);
        Assert.AreEqual(2, second.TravelerId);
        Assert.AreEqual(0, village.TravelerRecords.Count);
    }

    [Test]
    public void TravelerFactoryActivatesUnlockedTableAbilities()
    {
        VillageState village = new VillageState();
        village.TableProgress.UnlockPassive("memory_spark");
        village.TableProgress.UnlockActiveSkill("echo_call");

        TravelerRun traveler = TravelerFactory.CreateTraveler(village);

        Assert.Contains("memory_spark", traveler.ActiveTableAbilityIds);
        Assert.Contains("echo_call", traveler.ActiveTableAbilityIds);
    }

    [Test]
    public void GameFlowDeathCreatesRecordAndEchoForNextTraveler()
    {
        GameFlowController flow = new GameFlowController(new VillageState());

        TravelerRun first = flow.StartNewTraveler();
        flow.MarkTravelerDead("Battle loss", "mist-woods");
        TravelerRun second = flow.StartNewTraveler();

        Assert.AreEqual(GamePhase.Exploring, flow.Phase);
        Assert.AreEqual(1, flow.Village.TravelerRecords.Count);
        Assert.AreEqual(1, flow.Village.LegacyEchoes.Count);
        Assert.AreEqual(first.TravelerId + 1, second.TravelerId);
        Assert.AreEqual(1, second.VisibleLegacyEchoes.Count);
    }

    [Test]
    public void GameFlowCanResolveVisibleEchoForImmediateOrResearchReward()
    {
        GameFlowController flow = new GameFlowController(new VillageState());
        flow.StartNewTraveler();
        flow.MarkTravelerDead("Battle loss", "mist-woods");
        TravelerRun second = flow.StartNewTraveler();

        bool immediateResolved = flow.ResolveVisibleEcho(immediate: true);

        Assert.IsTrue(immediateResolved);
        Assert.AreEqual(55, second.MaxHp);
        Assert.AreEqual(0, flow.Village.TrainingLevel);

        flow.MarkTravelerDead("Battle loss", "mist-woods");
        flow.StartNewTraveler();

        bool researchResolved = flow.ResolveVisibleEcho(immediate: false);

        Assert.IsTrue(researchResolved);
        Assert.AreEqual(1, flow.Village.TrainingLevel);
    }

    [Test]
    public void GameFlowConsumesDefeatBattleResultAsDeathSettlement()
    {
        GameFlowController flow = new GameFlowController(new VillageState());
        flow.StartNewTraveler();
        BattleRunResult result = new BattleRunResult(false, true, 0, "Defeated in battle");

        flow.ApplyBattleResult(result, "mist-woods");

        Assert.AreEqual(GamePhase.RunSummary, flow.Phase);
        Assert.AreEqual(1, flow.Village.TravelerRecords.Count);
        Assert.AreEqual("Defeated in battle", flow.Village.TravelerRecords[0].DeathReason);
        Assert.AreEqual(1, flow.Village.LegacyEchoes.Count);
    }

    [Test]
    public void GameFlowRecordsConfirmedMapIntelWhenEnteringNode()
    {
        GameFlowController flow = new GameFlowController(new VillageState());
        TravelerRun traveler = flow.StartNewTraveler();
        MapNodeIntel node = flow.Exploration.Map.GetReachableNodes(flow.Exploration.CurrentNodeId)[0];

        flow.EnterNode(node.NodeId);
        flow.RecordConfirmedIntel(node);

        Assert.Contains(node.NodeId, traveler.ConfirmedIntelIds);
        Assert.AreEqual(1, flow.Village.MapIntelRecords.Count);
        Assert.AreEqual(node.NodeId, flow.Village.MapIntelRecords[0].NodeId);
    }
}
