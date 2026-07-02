using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnController
{
    private readonly BattleState state;
    private readonly BattleRandomRange randomRange;
    private readonly Action<string, Color> showEffect;

    public TurnController(BattleState state, BattleRandomRange randomRange)
        : this(state, randomRange, null)
    {
    }

    public TurnController(BattleState state, BattleRandomRange randomRange, Action<string, Color> showEffect)
    {
        this.state = state;
        this.randomRange = randomRange;
        this.showEffect = showEffect;
    }

    public void NewGame(BattleConfig config)
    {
        if (config == null)
        {
            NewGame(0, 0, 0, 0, null, null);
            return;
        }

        NewGame(config.PlayerMaxHp, config.MaxEnergy, config.HandSize, config.EnemyHandSize, config.StartingDeck, config.Stages);
    }

    public void NewGame(
        int playerMaxHp,
        int maxEnergy,
        int handSize,
        int enemyHandSize,
        IList<DeckEntry> startingDeck,
        IList<StageData> stages)
    {
        state.ClearBattle();
        state.PlayerMaxHp = playerMaxHp;
        state.MaxEnergy = maxEnergy;
        state.HandSize = handSize;
        state.EnemyHandSize = enemyHandSize;
        state.PlayerHp = playerMaxHp;

        if (stages != null)
        {
            for (int i = 0; i < stages.Count; i++)
            {
                if (stages[i] != null && stages[i].Enemy != null)
                {
                    state.Stages.Add(stages[i]);
                }
            }
        }

        if (startingDeck != null)
        {
            for (int i = 0; i < startingDeck.Count; i++)
            {
                DeckEntry entry = startingDeck[i];
                if (entry == null || entry.Card == null || entry.Count <= 0)
                {
                    continue;
                }

                state.PlayerDeck.AddToDrawPile(entry.Card, entry.Count);
            }
        }

        state.PlayerDeck.ShuffleDrawPile(randomRange);
        LoadStage(state.StageIndex);
        AddLog("战斗开始：打出卡牌，然后结束回合。");
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        state.Turn++;
        state.Energy = state.MaxEnergy;
        state.PlayerBlock = 0;
        PrepareEnemyHand();

        while (state.PlayerDeck.Hand.Count < state.HandSize)
        {
            if (!state.PlayerDeck.DrawToHand(randomRange, () => AddLog("弃牌堆已洗入牌库。")))
            {
                break;
            }
        }

        AddLog("第 " + state.Turn + " 回合：敌人准备打出 " + EnemyHandNames() + "。");
    }

    public bool PlayCard(int index)
    {
        if (state.GameOver || index < 0 || index >= state.PlayerDeck.Hand.Count)
        {
            return false;
        }

        CardData card = state.PlayerDeck.Hand[index];
        if (state.Energy < card.Cost)
        {
            AddLog("能量不足，无法打出「" + card.Name + "」。");
            return false;
        }

        state.Energy -= card.Cost;
        List<CardEffectData> effects = card.RuntimeEffects;
        for (int i = 0; i < effects.Count; i++)
        {
            ApplyCardEffect(card, effects[i]);
        }

        ShowEffect(BuildCardEffectText(card), card.Tint);
        state.PlayerDeck.Hand.RemoveAt(index);
        state.PlayerDeck.DiscardPile.Add(card);

        if (state.EnemyHp <= 0)
        {
            AdvanceStage();
        }

        return true;
    }

    public void EndTurn()
    {
        if (state.GameOver)
        {
            return;
        }

        state.PlayerDeck.MoveHandToDiscard();
        ResolveEnemyTurn();

        if (state.PlayerHp <= 0)
        {
            state.GameOver = true;
            AddLog("失败。点击新游戏再试一次。");
            return;
        }

        StartPlayerTurn();
    }

    public void ResolveEnemyTurn()
    {
        if (state.StageIndex < 0 || state.StageIndex >= state.Stages.Count)
        {
            return;
        }

        EnemyData enemy = state.Stages[state.StageIndex].Enemy;
        if (enemy == null)
        {
            return;
        }
        List<string> playedCards = new List<string>();
        state.EnemyBlock = 0;

        for (int i = 0; i < state.EnemyDeck.Hand.Count; i++)
        {
            EnemyActionData card = state.EnemyDeck.Hand[i];
            playedCards.Add(card.Name);

            if (card.Block > 0)
            {
                state.EnemyBlock += card.Block;
                AddLog("「" + enemy.Name + "」获得 " + card.Block + " 点格挡。");
            }

            if (card.Heal > 0)
            {
                state.EnemyHp = Mathf.Min(state.EnemyMaxHp, state.EnemyHp + card.Heal);
            }

            if (card.Damage > 0)
            {
                int damageTaken = Mathf.Max(0, card.Damage - state.PlayerBlock);
                state.PlayerBlock = Mathf.Max(0, state.PlayerBlock - card.Damage);
                state.PlayerHp = Mathf.Max(0, state.PlayerHp - damageTaken);
                AddLog("「" + enemy.Name + "」打出「" + card.Name + "」，造成 " + damageTaken + " 点伤害。");
            }
            else
            {
                AddLog("「" + enemy.Name + "」打出「" + card.Name + "」。");
            }
        }

        ShowEffect("敌人出牌\n" + string.Join("、", playedCards.ToArray()), enemy.Tint);
        state.EnemyDeck.MoveHandToDiscard();
    }

    private void PrepareEnemyHand()
    {
        state.EnemyDeck.MoveHandToDiscard();

        for (int i = 0; i < state.EnemyHandSize; i++)
        {
            state.EnemyDeck.DrawToHand(randomRange, null);
        }

        state.EnemyIntent = 0;
        for (int i = 0; i < state.EnemyDeck.Hand.Count; i++)
        {
            state.EnemyIntent += state.EnemyDeck.Hand[i].Damage;
        }
    }

    private void LoadStage(int index)
    {
        if (index < 0 || index >= state.Stages.Count)
        {
            state.ClearEnemy();
            return;
        }

        StageData stage = state.Stages[index];
        if (stage == null || stage.Enemy == null)
        {
            state.ClearEnemy();
            return;
        }

        EnemyData enemy = stage.Enemy;
        state.EnemyHp = enemy.MaxHp;
        state.EnemyMaxHp = enemy.MaxHp;
        state.EnemyBlock = 0;
        state.EnemyDeck.ClearAll();

        if (enemy.Cards == null)
        {
            enemy.Cards = new List<EnemyActionData>();
        }

        for (int i = 0; i < enemy.Cards.Count; i++)
        {
            EnemyActionData card = enemy.Cards[i];
            if (card == null || card.Copies <= 0)
            {
                continue;
            }

            state.EnemyDeck.AddToDrawPile(card, card.Copies);
        }

        state.EnemyDeck.ShuffleDrawPile(randomRange);
        AddLog("阶段 " + (state.StageIndex + 1) + " / " + state.Stages.Count + "：遭遇「" + enemy.Name + "」。");
    }

    private void AdvanceStage()
    {
        EnemyData defeated = state.Stages[state.StageIndex].Enemy;
        if (defeated == null)
        {
            state.ClearEnemy();
            return;
        }
        AddLog("击败「" + defeated.Name + "」！");
        state.StageIndex++;

        if (state.StageIndex >= state.Stages.Count)
        {
            state.GameOver = true;
            state.ClearEnemy();
            AddLog("胜利！你通过了全部阶段。");
            ShowEffect("全部通关！", new Color(0.96f, 0.88f, 0.35f));
            return;
        }

        LoadStage(state.StageIndex);
        PrepareEnemyHand();
        ShowEffect("进入阶段 " + (state.StageIndex + 1) + "\n" + state.Stages[state.StageIndex].Enemy.Name, state.Stages[state.StageIndex].Enemy.Tint);
    }

    private string EnemyHandNames()
    {
        if (state.EnemyDeck.Hand.Count == 0)
        {
            return "无";
        }

        List<string> names = new List<string>();
        for (int i = 0; i < state.EnemyDeck.Hand.Count; i++)
        {
            names.Add("「" + state.EnemyDeck.Hand[i].Name + "」");
        }

        return string.Join("、", names.ToArray());
    }

    private void ApplyCardEffect(CardData card, CardEffectData effect)
    {
        if (effect == null || effect.Amount <= 0)
        {
            return;
        }

        switch (effect.Type)
        {
            case CardEffectType.Damage:
                int damageDone = Mathf.Max(0, effect.Amount - state.EnemyBlock);
                state.EnemyBlock = Mathf.Max(0, state.EnemyBlock - effect.Amount);
                state.EnemyHp = Mathf.Max(0, state.EnemyHp - damageDone);
                AddLog("「" + card.Name + "」造成 " + damageDone + " 点伤害。");
                break;
            case CardEffectType.Block:
                state.PlayerBlock += effect.Amount;
                AddLog("「" + card.Name + "」获得 " + effect.Amount + " 点格挡。");
                break;
            case CardEffectType.Heal:
                state.PlayerHp = Mathf.Min(state.PlayerMaxHp, state.PlayerHp + effect.Amount);
                AddLog("「" + card.Name + "」恢复 " + effect.Amount + " 点生命。");
                break;
        }
    }

    private static string BuildCardEffectText(CardData card)
    {
        List<string> parts = new List<string>();
        List<CardEffectData> effects = card.RuntimeEffects;
        for (int i = 0; i < effects.Count; i++)
        {
            CardEffectData effect = effects[i];
            if (effect == null || effect.Amount <= 0)
            {
                continue;
            }

            switch (effect.Type)
            {
                case CardEffectType.Damage:
                    parts.Add("伤害 +" + effect.Amount);
                    break;
                case CardEffectType.Block:
                    parts.Add("格挡 +" + effect.Amount);
                    break;
                case CardEffectType.Heal:
                    parts.Add("生命 +" + effect.Amount);
                    break;
            }
        }

        return "打出「" + card.Name + "」\n" + string.Join("   ", parts.ToArray());
    }

    private void AddLog(string message)
    {
        state.LogLines.Enqueue(message);
        while (state.LogLines.Count > 7)
        {
            state.LogLines.Dequeue();
        }
    }

    private void ShowEffect(string message, Color color)
    {
        if (showEffect != null)
        {
            showEffect(message, color);
        }
    }
}
