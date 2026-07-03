using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class BattleFlowAdapterTests
{
    [Test]
    public void BattleAdapterReturnsVictoryResult()
    {
        CardData strike = CreateCard("Strike", 1, 8, 0, 0);
        StageData stage = CreateStage("Dummy", 8, new EnemyActionData("Wait", 0, 0, 0, "Wait", Color.white));
        BattleRunRequest request = new BattleRunRequest(
            30,
            30,
            3,
            1,
            0,
            new List<DeckEntry> { new DeckEntry(strike, 1) },
            new List<StageData> { stage });

        BattleRunResult result = BattleFlowAdapter.ResolveAuto(request, FixedShuffle, 5);

        Assert.IsTrue(result.IsVictory);
        Assert.IsFalse(result.IsDefeat);
        Assert.Greater(result.RemainingHp, 0);
        Assert.AreEqual(string.Empty, result.DeathReason);
    }

    [Test]
    public void BattleAdapterReturnsDefeatResultWithDeathReason()
    {
        CardData guard = CreateCard("Guard", 1, 0, 0, 0);
        StageData stage = CreateStage("Crusher", 20, new EnemyActionData("Crush", 50, 0, 0, "Deal 50", Color.white));
        BattleRunRequest request = new BattleRunRequest(
            20,
            10,
            3,
            1,
            1,
            new List<DeckEntry> { new DeckEntry(guard, 1) },
            new List<StageData> { stage });

        BattleRunResult result = BattleFlowAdapter.ResolveAuto(request, FixedShuffle, 3);

        Assert.IsFalse(result.IsVictory);
        Assert.IsTrue(result.IsDefeat);
        Assert.AreEqual(0, result.RemainingHp);
        Assert.AreEqual("Defeated in battle", result.DeathReason);
    }

    [Test]
    public void BattleAdapterTreatsTurnLimitAsRetreatDefeat()
    {
        CardData wait = CreateCard("Wait", 1, 0, 0, 0);
        StageData stage = CreateStage("Watcher", 20, new EnemyActionData("Watch", 0, 0, 0, "No damage", Color.white));
        BattleRunRequest request = new BattleRunRequest(
            20,
            20,
            3,
            1,
            1,
            new List<DeckEntry> { new DeckEntry(wait, 1) },
            new List<StageData> { stage });

        BattleRunResult result = BattleFlowAdapter.ResolveAuto(request, FixedShuffle, 2);

        Assert.IsFalse(result.IsVictory);
        Assert.IsTrue(result.IsDefeat);
        Assert.AreEqual("Battle timed out", result.DeathReason);
    }

    [Test]
    public void MemorySparkTableAbilityGivesBattleOpeningBlock()
    {
        CardData wait = CreateCard("Wait", 1, 0, 0, 0);
        StageData stage = CreateStage("Striker", 20, new EnemyActionData("Strike", 3, 0, 0, "Deal 3", Color.white));
        BattleRunRequest request = new BattleRunRequest(
            20,
            20,
            3,
            1,
            1,
            new List<DeckEntry> { new DeckEntry(wait, 1) },
            new List<StageData> { stage });
        request.TableAbilityIds.Add("memory_spark");

        BattleRunResult result = BattleFlowAdapter.ResolveAuto(request, FixedShuffle, 1);

        Assert.AreEqual(19, result.RemainingHp);
    }

    private static int FixedShuffle(int minInclusive, int maxExclusive)
    {
        return minInclusive;
    }

    private static CardData CreateCard(string name, int cost, int damage, int block, int heal)
    {
        CardData card = ScriptableObject.CreateInstance<CardData>();
        card.Name = name;
        card.Cost = cost;
        card.Damage = damage;
        card.Block = block;
        card.Heal = heal;
        card.Effects = CardEffectData.FromLegacyValues(damage, block, heal);
        card.Description = name;
        card.Tint = Color.white;
        return card;
    }

    private static StageData CreateStage(string enemyName, int maxHp, params EnemyActionData[] actions)
    {
        EnemyData enemy = ScriptableObject.CreateInstance<EnemyData>();
        enemy.Name = enemyName;
        enemy.MaxHp = maxHp;
        enemy.Tint = Color.gray;
        enemy.Cards = new List<EnemyActionData>(actions);

        StageData stage = ScriptableObject.CreateInstance<StageData>();
        stage.Enemy = enemy;
        return stage;
    }
}
