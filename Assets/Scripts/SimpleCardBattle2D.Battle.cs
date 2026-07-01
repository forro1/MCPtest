using System.Collections.Generic;
using UnityEngine;

public partial class SimpleCardBattle2D
{
    private void NewGame()
    {
        playerHp = playerMaxHp;
        playerBlock = 0;
        turn = 0;
        stageIndex = 0;
        gameOver = false;
        deck.Clear();
        discard.Clear();
        hand.Clear();
        logLines.Clear();

        for (int i = 0; i < startingDeck.Count; i++)
        {
            DeckEntry entry = startingDeck[i];
            if (entry == null || entry.Card == null || entry.Count <= 0)
            {
                continue;
            }

            AddToDeck(entry.Card, entry.Count);
        }

        Shuffle(deck);
        LoadStage(stageIndex);
        AddLog("战斗开始：打出卡牌，然后结束回合。");
        StartPlayerTurn();
    }

    private void AddToDeck(CardDef card, int count)
    {
        for (int i = 0; i < count; i++)
        {
            deck.Add(card);
        }
    }

    private void StartPlayerTurn()
    {
        turn++;
        energy = maxEnergy;
        playerBlock = 0;
        PrepareEnemyHand();
        while (hand.Count < handSize)
        {
            DrawCard();
        }
        AddLog("第 " + turn + " 回合：敌人准备打出 " + EnemyHandNames() + "。");
        RefreshUi();
    }

    private void DrawCard()
    {
        if (deck.Count == 0)
        {
            if (discard.Count == 0)
            {
                return;
            }
            deck.AddRange(discard);
            discard.Clear();
            Shuffle(deck);
            AddLog("弃牌堆已洗入牌库。");
        }

        hand.Add(deck[0]);
        deck.RemoveAt(0);
    }

    private void PrepareEnemyHand()
    {
        enemyDiscard.AddRange(enemyHand);
        enemyHand.Clear();

        for (int i = 0; i < enemyHandSize; i++)
        {
            DrawEnemyCard();
        }

        enemyIntent = 0;
        for (int i = 0; i < enemyHand.Count; i++)
        {
            enemyIntent += enemyHand[i].Damage;
        }
    }

    private void DrawEnemyCard()
    {
        if (enemyDeck.Count == 0)
        {
            if (enemyDiscard.Count == 0)
            {
                return;
            }

            enemyDeck.AddRange(enemyDiscard);
            enemyDiscard.Clear();
            Shuffle(enemyDeck);
        }

        enemyHand.Add(enemyDeck[0]);
        enemyDeck.RemoveAt(0);
    }

    private void PlayCard(int index)
    {
        if (gameOver || index < 0 || index >= hand.Count)
        {
            return;
        }

        CardDef card = hand[index];
        if (energy < card.Cost)
        {
            AddLog("能量不足，无法打出「" + card.Name + "」。");
            RefreshUi();
            return;
        }

        energy -= card.Cost;
        if (card.Damage > 0)
        {
            int damageDone = Mathf.Max(0, card.Damage - enemyBlock);
            enemyBlock = Mathf.Max(0, enemyBlock - card.Damage);
            enemyHp = Mathf.Max(0, enemyHp - damageDone);
            AddLog("「" + card.Name + "」造成 " + damageDone + " 点伤害。");
        }
        if (card.Block > 0)
        {
            playerBlock += card.Block;
            AddLog("「" + card.Name + "」获得 " + card.Block + " 点格挡。");
        }
        if (card.Heal > 0)
        {
            playerHp = Mathf.Min(playerMaxHp, playerHp + card.Heal);
            AddLog("「" + card.Name + "」恢复 " + card.Heal + " 点生命。");
        }

        ShowCardEffect(BuildCardEffectText(card), card.Tint);

        hand.RemoveAt(index);
        discard.Add(card);

        if (enemyHp <= 0)
        {
            AdvanceStage();
        }

        RefreshUi();
    }

    private void EndTurn()
    {
        if (gameOver)
        {
            NewGame();
            return;
        }

        discard.AddRange(hand);
        hand.Clear();

        ResolveEnemyTurn();

        if (playerHp <= 0)
        {
            gameOver = true;
            AddLog("失败。点击新游戏再试一次。");
            RefreshUi();
            return;
        }

        StartPlayerTurn();
    }

    private void ResolveEnemyTurn()
    {
        EnemyDef enemy = stages[stageIndex];
        List<string> playedCards = new List<string>();
        enemyBlock = 0;

        for (int i = 0; i < enemyHand.Count; i++)
        {
            EnemyCardDef card = enemyHand[i];
            playedCards.Add(card.Name);

            if (card.Block > 0)
            {
                enemyBlock += card.Block;
                AddLog("「" + enemy.Name + "」获得 " + card.Block + " 点格挡。");
            }

            if (card.Heal > 0)
            {
                enemyHp = Mathf.Min(enemyMaxHp, enemyHp + card.Heal);
            }

            if (card.Damage > 0)
            {
                int damageTaken = Mathf.Max(0, card.Damage - playerBlock);
                playerBlock = Mathf.Max(0, playerBlock - card.Damage);
                playerHp = Mathf.Max(0, playerHp - damageTaken);
                AddLog("「" + enemy.Name + "」打出「" + card.Name + "」，造成 " + damageTaken + " 点伤害。");
            }
            else
            {
                AddLog("「" + enemy.Name + "」打出「" + card.Name + "」。");
            }
        }

        ShowCardEffect("敌人出牌\n" + string.Join("、", playedCards.ToArray()), enemy.Tint);
        enemyDiscard.AddRange(enemyHand);
        enemyHand.Clear();
    }

    private void AdvanceStage()
    {
        EnemyDef defeated = stages[stageIndex];
        AddLog("击败「" + defeated.Name + "」！");
        stageIndex++;

        if (stageIndex >= stages.Count)
        {
            gameOver = true;
            AddLog("胜利！你通过了全部阶段。");
            ShowCardEffect("全部通关！", new Color(0.96f, 0.88f, 0.35f));
            return;
        }

        LoadStage(stageIndex);
        PrepareEnemyHand();
        ShowCardEffect("进入阶段 " + (stageIndex + 1) + "\n" + stages[stageIndex].Name, stages[stageIndex].Tint);
    }
}
