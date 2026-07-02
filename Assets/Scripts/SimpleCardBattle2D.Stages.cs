using UnityEngine;

public partial class SimpleCardBattle2D
{
    [ContextMenu("填充默认战斗配置")]
    private void FillDefaultBattleConfig()
    {
        startingDeck.Clear();
        stages.Clear();
        AddDefaultStartingDeck();
        AddDefaultStages();
    }

    private void InitializeStages()
    {
        if (battleConfig != null)
        {
            return;
        }

        if (startingDeck.Count == 0)
        {
            AddDefaultStartingDeck();
        }

        if (stages.Count > 0)
        {
            return;
        }

        AddDefaultStages();
    }

    private void AddDefaultStartingDeck()
    {
        startingDeck.Add(new DeckEntry(CreateRuntimeCard("斩击", 1, 6, 0, 0, "造成6点伤害", new Color(0.90f, 0.34f, 0.30f), "Cards/card_strike_attack"), 5));
        startingDeck.Add(new DeckEntry(CreateRuntimeCard("格挡", 1, 0, 6, 0, "获得6点格挡", new Color(0.30f, 0.52f, 0.86f), "Cards/card_guard_defense"), 4));
        startingDeck.Add(new DeckEntry(CreateRuntimeCard("火花", 0, 3, 0, 0, "造成3点伤害", new Color(0.95f, 0.76f, 0.28f), "Cards/card_spark_fire"), 2));
        startingDeck.Add(new DeckEntry(CreateRuntimeCard("治疗", 1, 0, 0, 5, "恢复5点生命", new Color(0.35f, 0.74f, 0.45f), "Cards/card_mend_heal"), 1));
        startingDeck.Add(new DeckEntry(CreateRuntimeCard("重击", 2, 12, 0, 0, "造成12点伤害", new Color(0.72f, 0.36f, 0.76f), "Cards/card_bash_heavy"), 1));
    }

    private void AddDefaultStages()
    {
        stages.Add(CreateRuntimeStage(CreateRuntimeEnemy(
            "训练假人",
            42,
            new Color(0.72f, 0.68f, 0.56f),
            "Enemies/training_dummy",
            new EnemyActionData("木槌", 7, 0, 0, "造成7点伤害", new Color(0.80f, 0.58f, 0.34f)),
            new EnemyActionData("硬化木皮", 0, 5, 0, "获得5点格挡", new Color(0.58f, 0.66f, 0.44f)),
            new EnemyActionData("笨拙撞击", 9, 0, 0, "造成9点伤害", new Color(0.88f, 0.48f, 0.30f)))));

        stages.Add(CreateRuntimeStage(CreateRuntimeEnemy(
            "毒刃盗贼",
            48,
            new Color(0.42f, 0.82f, 0.48f),
            "Enemies/poison_rogue",
            new EnemyActionData("毒刃", 6, 0, 0, "造成6点伤害", new Color(0.35f, 0.85f, 0.42f)),
            new EnemyActionData("闪避", 0, 8, 0, "获得8点格挡", new Color(0.38f, 0.62f, 0.90f)),
            new EnemyActionData("背刺", 11, 0, 0, "造成11点伤害", new Color(0.84f, 0.34f, 0.38f)))));

        stages.Add(CreateRuntimeStage(CreateRuntimeEnemy(
            "石甲守卫",
            60,
            new Color(0.62f, 0.66f, 0.74f),
            "Enemies/stone_guardian",
            new EnemyActionData("盾击", 8, 4, 0, "造成8点伤害并获得4点格挡", new Color(0.54f, 0.58f, 0.72f)),
            new EnemyActionData("石肤", 0, 12, 0, "获得12点格挡", new Color(0.44f, 0.48f, 0.58f)),
            new EnemyActionData("重碾", 14, 0, 0, "造成14点伤害", new Color(0.72f, 0.42f, 0.34f)))));

        stages.Add(CreateRuntimeStage(CreateRuntimeEnemy(
            "烈焰术士",
            54,
            new Color(0.95f, 0.42f, 0.22f),
            "Enemies/flame_warlock",
            new EnemyActionData("火球", 12, 0, 0, "造成12点伤害", new Color(0.96f, 0.38f, 0.20f)),
            new EnemyActionData("火焰护盾", 0, 7, 0, "获得7点格挡", new Color(0.92f, 0.55f, 0.18f)),
            new EnemyActionData("汲取余烬", 5, 0, 5, "造成5点伤害并恢复5点生命", new Color(0.92f, 0.30f, 0.48f)))));
    }

    private static CardData CreateRuntimeCard(string name, int cost, int damage, int block, int heal, string description, Color tint, string artPath)
    {
        CardData card = ScriptableObject.CreateInstance<CardData>();
        card.Name = name;
        card.Cost = cost;
        card.Damage = damage;
        card.Block = block;
        card.Heal = heal;
        card.Description = description;
        card.Tint = tint;
        card.ArtPath = artPath;
        return card;
    }

    private static EnemyData CreateRuntimeEnemy(string name, int maxHp, Color tint, string artPath, params EnemyActionData[] cards)
    {
        EnemyData enemy = ScriptableObject.CreateInstance<EnemyData>();
        enemy.Name = name;
        enemy.MaxHp = maxHp;
        enemy.Tint = tint;
        enemy.ArtPath = artPath;
        enemy.Cards = new System.Collections.Generic.List<EnemyActionData>(cards);
        return enemy;
    }

    private static StageData CreateRuntimeStage(EnemyData enemy)
    {
        StageData stage = ScriptableObject.CreateInstance<StageData>();
        stage.Enemy = enemy;
        return stage;
    }
}
