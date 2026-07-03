using System.Collections.Generic;
using UnityEngine;

public static class BattleRunRequestFactory
{
    public static BattleRunRequest CreatePhaseOneRequest(TravelerRun traveler, MapNodeIntel node)
    {
        int maxHp = traveler == null ? TravelerFactory.BaseMaxHp : traveler.MaxHp;
        int currentHp = traveler == null ? maxHp : traveler.CurrentHp;
        List<DeckEntry> deck = BuildDeck(traveler);
        List<StageData> stages = new List<StageData> { BuildStage(node) };
        BattleRunRequest request = new BattleRunRequest(maxHp, currentHp, 3, 3, 1, deck, stages);
        if (traveler != null)
        {
            request.TableAbilityIds.AddRange(traveler.ActiveTableAbilityIds);
        }

        return request;
    }

    private static List<DeckEntry> BuildDeck(TravelerRun traveler)
    {
        List<string> cardIds = traveler == null ? new List<string> { "Strike", "Guard", "Spark" } : traveler.DeckCardIds;
        List<DeckEntry> deck = new List<DeckEntry>();
        for (int i = 0; i < cardIds.Count; i++)
        {
            deck.Add(new DeckEntry(CreateRuntimeCard(cardIds[i]), 1));
        }

        return deck;
    }

    private static CardData CreateRuntimeCard(string cardId)
    {
        switch (cardId)
        {
            case "Guard":
                return CreateCard("Guard", 1, 0, 6, 0, "Gain 6 block", new Color(0.30f, 0.52f, 0.86f));
            case "Spark":
                return CreateCard("Spark", 0, 3, 0, 0, "Deal 3 damage", new Color(0.95f, 0.76f, 0.28f));
            case "EchoStrike":
                return CreateCard("Echo Strike", 1, 9, 0, 0, "Deal 9 damage", new Color(0.70f, 0.42f, 0.84f));
            case "Strike":
            default:
                return CreateCard("Strike", 1, 6, 0, 0, "Deal 6 damage", new Color(0.90f, 0.34f, 0.30f));
        }
    }

    private static StageData BuildStage(MapNodeIntel node)
    {
        int risk = node == null ? 2 : node.ActualRiskLevel;
        int reward = node == null ? 1 : node.ActualRewardLevel;
        EnemyData enemy = ScriptableObject.CreateInstance<EnemyData>();
        enemy.Name = node == null ? "Mist Echo" : "Mist Echo " + node.RegionId;
        enemy.MaxHp = 14 + risk * 5 + reward * 2;
        enemy.Tint = Color.gray;
        enemy.Cards = new List<EnemyActionData>
        {
            new EnemyActionData("Pressure", 4 + risk * 2, 0, 0, "Deal damage based on actual risk", Color.white),
            new EnemyActionData("Brace", 0, 3 + reward, 0, "Gain block", Color.white)
        };

        StageData stage = ScriptableObject.CreateInstance<StageData>();
        stage.Enemy = enemy;
        return stage;
    }

    private static CardData CreateCard(string name, int cost, int damage, int block, int heal, string description, Color tint)
    {
        CardData card = ScriptableObject.CreateInstance<CardData>();
        card.Name = name;
        card.Cost = cost;
        card.Damage = damage;
        card.Block = block;
        card.Heal = heal;
        card.Description = description;
        card.Effects = CardEffectData.FromLegacyValues(damage, block, heal);
        card.Tint = tint;
        return card;
    }
}
