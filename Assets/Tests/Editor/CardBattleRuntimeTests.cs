using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CardBattleRuntimeTests
{
    [Test]
    public void PlayCardConsumesEnergyMovesCardAndDamagesThroughEnemyBlock()
    {
        CardData strike = CreateCard("Strike", 1, 6, 0, 0);
        StageData stage = CreateStage("Dummy", 20, new EnemyActionData("Wait", 0, 0, 0, "Wait", Color.white));

        BattleState state = new BattleState();
        TurnController controller = new TurnController(state, FixedShuffle);

        controller.NewGame(
            30,
            3,
            1,
            0,
            new List<DeckEntry> { new DeckEntry(strike, 1) },
            new List<StageData> { stage });
        state.EnemyBlock = 2;

        controller.PlayCard(0);

        Assert.AreEqual(2, state.Energy);
        Assert.AreEqual(16, state.EnemyHp);
        Assert.AreEqual(0, state.EnemyBlock);
        Assert.AreEqual(0, state.PlayerDeck.Hand.Count);
        Assert.AreEqual(1, state.PlayerDeck.DiscardPile.Count);
    }

    [Test]
    public void EndTurnMovesHandToDiscardAndStartsNextPlayerTurn()
    {
        CardData defend = CreateCard("Defend", 1, 0, 5, 0);
        StageData stage = CreateStage("Dummy", 20, new EnemyActionData("Hit", 3, 0, 0, "Deal 3", Color.white));

        BattleState state = new BattleState();
        TurnController controller = new TurnController(state, FixedShuffle);

        controller.NewGame(
            30,
            3,
            1,
            1,
            new List<DeckEntry> { new DeckEntry(defend, 2) },
            new List<StageData> { stage });

        controller.EndTurn();

        Assert.AreEqual(2, state.Turn);
        Assert.AreEqual(27, state.PlayerHp);
        Assert.AreEqual(1, state.PlayerDeck.Hand.Count);
        Assert.AreEqual(1, state.PlayerDeck.DiscardPile.Count);
    }

    [Test]
    public void NewGameCanStartFromBattleConfigAsset()
    {
        CardData card = CreateCard("Config Strike", 1, 4, 0, 0);
        StageData stage = CreateStage("Configured Dummy", 12, new EnemyActionData("Wait", 0, 0, 0, "Wait", Color.white));
        BattleConfig config = ScriptableObject.CreateInstance<BattleConfig>();
        config.PlayerMaxHp = 44;
        config.MaxEnergy = 4;
        config.HandSize = 1;
        config.EnemyHandSize = 0;
        config.StartingDeck = new List<DeckEntry> { new DeckEntry(card, 1) };
        config.Stages = new List<StageData> { stage };

        BattleState state = new BattleState();
        TurnController controller = new TurnController(state, FixedShuffle);

        controller.NewGame(config);

        Assert.AreEqual(44, state.PlayerMaxHp);
        Assert.AreEqual(4, state.MaxEnergy);
        Assert.AreEqual(1, state.PlayerDeck.Hand.Count);
        Assert.AreEqual(stage, state.Stages[0]);
    }

    [Test]
    public void CardEffectListAllowsNewCardsWithoutControllerChanges()
    {
        CardData combo = ScriptableObject.CreateInstance<CardData>();
        combo.Name = "Combo";
        combo.Cost = 1;
        combo.Effects = new List<CardEffectData>
        {
            new CardEffectData(CardEffectType.Damage, 5),
            new CardEffectData(CardEffectType.Block, 3),
            new CardEffectData(CardEffectType.Heal, 2)
        };
        StageData stage = CreateStage("Dummy", 20, new EnemyActionData("Wait", 0, 0, 0, "Wait", Color.white));

        BattleState state = new BattleState();
        TurnController controller = new TurnController(state, FixedShuffle);
        controller.NewGame(10, 3, 1, 0, new List<DeckEntry> { new DeckEntry(combo, 1) }, new List<StageData> { stage });
        state.PlayerHp = 7;

        controller.PlayCard(0);

        Assert.AreEqual(15, state.EnemyHp);
        Assert.AreEqual(3, state.PlayerBlock);
        Assert.AreEqual(9, state.PlayerHp);
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
