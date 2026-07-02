using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Battle/Battle Config", fileName = "BattleConfig")]
public class BattleConfig : ScriptableObject
{
    [Header("玩家数值")]
    [Min(1)]
    public int PlayerMaxHp = 50;
    [Min(0)]
    public int MaxEnergy = 3;
    [Min(0)]
    public int HandSize = 5;

    [Header("敌人数值")]
    [Min(0)]
    public int EnemyHandSize = 2;

    [Header("初始牌组")]
    public List<DeckEntry> StartingDeck = new List<DeckEntry>();

    [Header("关卡配置")]
    public List<StageData> Stages = new List<StageData>();
}
