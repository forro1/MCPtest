using System.Collections.Generic;

public class BattleState
{
    public int PlayerMaxHp;
    public int MaxEnergy;
    public int HandSize;
    public int EnemyHandSize;

    public int PlayerHp;
    public int EnemyHp;
    public int EnemyMaxHp;
    public int EnemyBlock;
    public int PlayerBlock;
    public int Energy;
    public int EnemyIntent;
    public int Turn;
    public int StageIndex;
    public bool GameOver;

    public readonly DeckRuntime<CardData> PlayerDeck = new DeckRuntime<CardData>();
    public readonly DeckRuntime<EnemyActionData> EnemyDeck = new DeckRuntime<EnemyActionData>();
    public readonly List<StageData> Stages = new List<StageData>();
    public readonly Queue<string> LogLines = new Queue<string>();

    public void ClearBattle()
    {
        PlayerDeck.ClearAll();
        EnemyDeck.ClearAll();
        Stages.Clear();
        LogLines.Clear();

        PlayerBlock = 0;
        EnemyBlock = 0;
        Energy = 0;
        EnemyIntent = 0;
        Turn = 0;
        StageIndex = 0;
        GameOver = false;
    }

    public void ClearEnemy()
    {
        EnemyHp = 0;
        EnemyMaxHp = 0;
        EnemyBlock = 0;
        EnemyIntent = 0;
        EnemyDeck.ClearAll();
    }
}
