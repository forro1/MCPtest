using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class SimpleCardBattle2D
{
    [Header("战斗配置")]
    [SerializeField] private BattleConfig battleConfig;

    [Header("玩家数值")]
    [Min(1)]
    [SerializeField] private int playerMaxHp = 50;
    [Min(0)]
    [SerializeField] private int maxEnergy = 3;
    [Min(0)]
    [SerializeField] private int handSize = 5;

    [Header("敌人数值")]
    [Min(0)]
    [SerializeField] private int enemyHandSize = 2;

    [Header("初始牌组")]
    [SerializeField] private List<DeckEntry> startingDeck = new List<DeckEntry>();

    [Header("关卡配置")]
    [SerializeField] private List<StageData> stages = new List<StageData>();

    private readonly BattleState battleState = new BattleState();
    private readonly Dictionary<string, Sprite> enemySprites = new Dictionary<string, Sprite>();
    private readonly Dictionary<string, Sprite> cardSprites = new Dictionary<string, Sprite>();
    private TurnController turnController;

    private Transform handRoot;
    private Text playerText;
    private Text enemyText;
    private Text enemyHandText;
    private Image enemyArtImage;
    private Text statusText;
    private Text logText;
    private Text drawPileText;
    private Text discardPileText;
    private Text effectText;
    private Button endTurnButton;
    private Font uiFont;
    private Coroutine effectRoutine;

}
