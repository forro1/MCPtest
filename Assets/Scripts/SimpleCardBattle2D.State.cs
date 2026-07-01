using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class SimpleCardBattle2D
{
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
    [SerializeField] private List<EnemyDef> stages = new List<EnemyDef>();

    private readonly List<CardDef> deck = new List<CardDef>();
    private readonly List<CardDef> discard = new List<CardDef>();
    private readonly List<CardDef> hand = new List<CardDef>();
    private readonly List<EnemyCardDef> enemyDeck = new List<EnemyCardDef>();
    private readonly List<EnemyCardDef> enemyDiscard = new List<EnemyCardDef>();
    private readonly List<EnemyCardDef> enemyHand = new List<EnemyCardDef>();
    private readonly Dictionary<string, Sprite> enemySprites = new Dictionary<string, Sprite>();
    private readonly Dictionary<string, Sprite> cardSprites = new Dictionary<string, Sprite>();
    private readonly Queue<string> logLines = new Queue<string>();

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

    private int playerHp;
    private int enemyHp;
    private int enemyMaxHp;
    private int enemyBlock;
    private int playerBlock;
    private int energy;
    private int enemyIntent;
    private int turn;
    private int stageIndex;
    private bool gameOver;
}
