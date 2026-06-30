using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleCardBattle2D : MonoBehaviour
{
    private class CardDef
    {
        public string Name;
        public int Cost;
        public int Damage;
        public int Block;
        public int Heal;
        public string Description;
        public Color Tint;
        public string ArtPath;

        public CardDef(string name, int cost, int damage, int block, int heal, string description, Color tint, string artPath)
        {
            Name = name;
            Cost = cost;
            Damage = damage;
            Block = block;
            Heal = heal;
            Description = description;
            Tint = tint;
            ArtPath = artPath;
        }
    }

    private class EnemyCardDef
    {
        public string Name;
        public int Damage;
        public int Block;
        public int Heal;
        public string Description;
        public Color Tint;

        public EnemyCardDef(string name, int damage, int block, int heal, string description, Color tint)
        {
            Name = name;
            Damage = damage;
            Block = block;
            Heal = heal;
            Description = description;
            Tint = tint;
        }
    }

    private class EnemyDef
    {
        public string Name;
        public int MaxHp;
        public Color Tint;
        public string ArtPath;
        public List<EnemyCardDef> Cards;

        public EnemyDef(string name, int maxHp, Color tint, string artPath, params EnemyCardDef[] cards)
        {
            Name = name;
            MaxHp = maxHp;
            Tint = tint;
            ArtPath = artPath;
            Cards = new List<EnemyCardDef>(cards);
        }
    }

    private readonly List<CardDef> deck = new List<CardDef>();
    private readonly List<CardDef> discard = new List<CardDef>();
    private readonly List<CardDef> hand = new List<CardDef>();
    private readonly List<EnemyDef> stages = new List<EnemyDef>();
    private readonly List<EnemyCardDef> enemyDeck = new List<EnemyCardDef>();
    private readonly List<EnemyCardDef> enemyDiscard = new List<EnemyCardDef>();
    private readonly List<EnemyCardDef> enemyHand = new List<EnemyCardDef>();
    private readonly Dictionary<string, Sprite> enemySprites = new Dictionary<string, Sprite>();
    private readonly Dictionary<string, Sprite> cardSprites = new Dictionary<string, Sprite>();
    private readonly Queue<string> logLines = new Queue<string>();

    private readonly CardDef strike = new CardDef("斩击", 1, 6, 0, 0, "造成6点伤害", new Color(0.90f, 0.34f, 0.30f), "Cards/card_strike_attack");
    private readonly CardDef guard = new CardDef("格挡", 1, 0, 6, 0, "获得6点格挡", new Color(0.30f, 0.52f, 0.86f), "Cards/card_guard_defense");
    private readonly CardDef spark = new CardDef("火花", 0, 3, 0, 0, "造成3点伤害", new Color(0.95f, 0.76f, 0.28f), "Cards/card_spark_fire");
    private readonly CardDef mend = new CardDef("治疗", 1, 0, 0, 5, "恢复5点生命", new Color(0.35f, 0.74f, 0.45f), "Cards/card_mend_heal");
    private readonly CardDef bash = new CardDef("重击", 2, 12, 0, 0, "造成12点伤害", new Color(0.72f, 0.36f, 0.76f), "Cards/card_bash_heavy");

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

    private const int PlayerMaxHp = 50;
    private const int MaxEnergy = 3;
    private const int HandSize = 5;
    private const int EnemyHandSize = 2;

    private void Start()
    {
        UnityEngine.Random.InitState(Environment.TickCount);
        BuildUi();
        InitializeStages();
        NewGame();
    }

    private void NewGame()
    {
        playerHp = PlayerMaxHp;
        playerBlock = 0;
        turn = 0;
        stageIndex = 0;
        gameOver = false;
        deck.Clear();
        discard.Clear();
        hand.Clear();
        logLines.Clear();

        AddToDeck(strike, 5);
        AddToDeck(guard, 4);
        AddToDeck(spark, 2);
        AddToDeck(mend, 1);
        AddToDeck(bash, 1);
        Shuffle(deck);
        LoadStage(stageIndex);
        AddLog("战斗开始：打出卡牌，然后结束回合。");
        StartPlayerTurn();
    }

    private void InitializeStages()
    {
        stages.Clear();
        stages.Add(new EnemyDef(
            "训练假人",
            42,
            new Color(0.72f, 0.68f, 0.56f),
            "Enemies/training_dummy",
            new EnemyCardDef("木槌", 7, 0, 0, "造成7点伤害", new Color(0.80f, 0.58f, 0.34f)),
            new EnemyCardDef("硬化木皮", 0, 5, 0, "获得5点格挡", new Color(0.58f, 0.66f, 0.44f)),
            new EnemyCardDef("笨拙撞击", 9, 0, 0, "造成9点伤害", new Color(0.88f, 0.48f, 0.30f))));

        stages.Add(new EnemyDef(
            "毒刃盗贼",
            48,
            new Color(0.42f, 0.82f, 0.48f),
            "Enemies/poison_rogue",
            new EnemyCardDef("毒刃", 6, 0, 0, "造成6点伤害", new Color(0.35f, 0.85f, 0.42f)),
            new EnemyCardDef("闪避", 0, 8, 0, "获得8点格挡", new Color(0.38f, 0.62f, 0.90f)),
            new EnemyCardDef("背刺", 11, 0, 0, "造成11点伤害", new Color(0.84f, 0.34f, 0.38f))));

        stages.Add(new EnemyDef(
            "石甲守卫",
            60,
            new Color(0.62f, 0.66f, 0.74f),
            "Enemies/stone_guardian",
            new EnemyCardDef("盾击", 8, 4, 0, "造成8点伤害并获得4点格挡", new Color(0.54f, 0.58f, 0.72f)),
            new EnemyCardDef("石肤", 0, 12, 0, "获得12点格挡", new Color(0.44f, 0.48f, 0.58f)),
            new EnemyCardDef("重碾", 14, 0, 0, "造成14点伤害", new Color(0.72f, 0.42f, 0.34f))));

        stages.Add(new EnemyDef(
            "烈焰术士",
            54,
            new Color(0.95f, 0.42f, 0.22f),
            "Enemies/flame_warlock",
            new EnemyCardDef("火球", 12, 0, 0, "造成12点伤害", new Color(0.96f, 0.38f, 0.20f)),
            new EnemyCardDef("火焰护盾", 0, 7, 0, "获得7点格挡", new Color(0.92f, 0.55f, 0.18f)),
            new EnemyCardDef("汲取余烬", 5, 0, 5, "造成5点伤害并恢复5点生命", new Color(0.92f, 0.30f, 0.48f))));
    }

    private void LoadStage(int index)
    {
        EnemyDef enemy = stages[index];
        enemyHp = enemy.MaxHp;
        enemyMaxHp = enemy.MaxHp;
        enemyBlock = 0;
        enemyDeck.Clear();
        enemyDiscard.Clear();
        enemyHand.Clear();

        for (int i = 0; i < enemy.Cards.Count; i++)
        {
            enemyDeck.Add(enemy.Cards[i]);
            enemyDeck.Add(enemy.Cards[i]);
        }

        Shuffle(enemyDeck);
        AddLog("阶段 " + (stageIndex + 1) + " / " + stages.Count + "：遭遇「" + enemy.Name + "」。");
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
        energy = MaxEnergy;
        playerBlock = 0;
        enemyBlock = 0;
        PrepareEnemyHand();
        while (hand.Count < HandSize)
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

        for (int i = 0; i < EnemyHandSize; i++)
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
            playerHp = Mathf.Min(PlayerMaxHp, playerHp + card.Heal);
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

        for (int i = 0; i < enemyHand.Count; i++)
        {
            EnemyCardDef card = enemyHand[i];
            playedCards.Add(card.Name);

            if (card.Block > 0)
            {
                enemyBlock += card.Block;
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

    private void BuildUi()
    {
        uiFont = Font.CreateDynamicFontFromOSFont(new[] { "Microsoft YaHei", "SimHei", "Arial" }, 18);

        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem));
            Type inputSystemModuleType = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSystemModuleType != null)
            {
                eventSystem.AddComponent(inputSystemModuleType);
            }
            else
            {
                eventSystem.AddComponent<StandaloneInputModule>();
            }
        }

        GameObject canvasObject = new GameObject("Card Battle Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        Image background = CreatePanel(canvasObject.transform, "Table", AnchorStretch(), new Color(0.10f, 0.12f, 0.16f));
        background.raycastTarget = false;

        Text title = CreateText(canvasObject.transform, "Title", "秘法牌桌", 34, FontStyle.Bold, new Color(0.96f, 0.88f, 0.68f));
        SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -60f), new Vector2(760f, 72f));

        enemyText = CreateText(canvasObject.transform, "Enemy Panel", string.Empty, 25, FontStyle.Bold, Color.white);
        SetRect(enemyText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -150f), new Vector2(760f, 96f));

        enemyHandText = CreateText(canvasObject.transform, "Enemy Hand", string.Empty, 18, FontStyle.Bold, new Color(0.94f, 0.78f, 0.52f));
        SetRect(enemyHandText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -250f), new Vector2(1120f, 58f));
        enemyHandText.alignment = TextAnchor.MiddleCenter;
        enemyHandText.resizeTextForBestFit = true;
        enemyHandText.resizeTextMinSize = 13;
        enemyHandText.resizeTextMaxSize = 18;
        

        enemyArtImage = CreateImage(canvasObject.transform, "Enemy Art", new Color(1f, 1f, 1f, 1f));
        SetRect(enemyArtImage.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 0.5f), new Vector2(0f, -530f), new Vector2(380f, 430f));

        playerText = CreateText(canvasObject.transform, "Player Panel", string.Empty, 23, FontStyle.Bold, Color.white);
        SetRect(playerText.rectTransform, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(46f, 260f), new Vector2(460f, 96f));
        playerText.alignment = TextAnchor.MiddleLeft;

        statusText = CreateText(canvasObject.transform, "Status", string.Empty, 22, FontStyle.Bold, new Color(0.95f, 0.95f, 0.92f));
        SetRect(statusText.rectTransform, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-46f, 260f), new Vector2(460f, 96f));
        statusText.alignment = TextAnchor.MiddleRight;

        logText = CreateText(canvasObject.transform, "Battle Log", string.Empty, 17, FontStyle.Normal, new Color(0.84f, 0.88f, 0.94f));
        SetRect(logText.rectTransform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-42f, -42f), new Vector2(560f, 300f));
        logText.alignment = TextAnchor.UpperLeft;
        Image logPanel = CreatePanel(logText.transform.parent, "Log Backdrop", RectFrom(logText.rectTransform), new Color(0.05f, 0.06f, 0.08f, 0.72f));
        logPanel.transform.SetSiblingIndex(logText.transform.GetSiblingIndex());

        drawPileText = CreateSmallPile(canvasObject.transform, "牌库", new Vector2(42f, 60f));
        discardPileText = CreateSmallPile(canvasObject.transform, "弃牌", new Vector2(198f, 60f));

        GameObject handObject = new GameObject("Hand", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        handObject.transform.SetParent(canvasObject.transform, false);
        handRoot = handObject.transform;
        RectTransform handRect = handObject.GetComponent<RectTransform>();
        SetRect(handRect, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 38f), new Vector2(980f, 240f));
        HorizontalLayoutGroup layout = handObject.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = 18f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlHeight = false;
        layout.childControlWidth = false;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = false;

        endTurnButton = CreateButton(canvasObject.transform, "End Turn", "结束回合", new Color(0.88f, 0.55f, 0.22f));
        SetRect(endTurnButton.GetComponent<RectTransform>(), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-42f, 60f), new Vector2(220f, 72f));
        endTurnButton.onClick.AddListener(EndTurn);

        Button newGameButton = CreateButton(canvasObject.transform, "New Game", "新游戏", new Color(0.28f, 0.58f, 0.42f));
        SetRect(newGameButton.GetComponent<RectTransform>(), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-42f, 152f), new Vector2(220f, 64f));
        newGameButton.onClick.AddListener(NewGame);

        effectText = CreateText(canvasObject.transform, "Play Effect", string.Empty, 30, FontStyle.Bold, Color.white);
        SetRect(effectText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 40f), new Vector2(760f, 130f));
        effectText.alignment = TextAnchor.MiddleCenter;
        effectText.raycastTarget = false;
        effectText.canvasRenderer.SetAlpha(0f);
    }

    private Text CreateSmallPile(Transform parent, string label, Vector2 anchoredPosition)
    {
        Text text = CreateText(parent, label + " Text", string.Empty, 18, FontStyle.Bold, Color.white);
        SetRect(text.rectTransform, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), anchoredPosition, new Vector2(130f, 96f));
        Image panel = CreatePanel(parent, label + " Pile", RectFrom(text.rectTransform), new Color(0.16f, 0.18f, 0.24f));
        panel.transform.SetSiblingIndex(text.transform.GetSiblingIndex());
        return text;
    }

    private void RefreshUi()
    {
        EnemyDef enemy = stages[Mathf.Clamp(stageIndex, 0, stages.Count - 1)];
        int stageDisplay = Mathf.Min(stageIndex + 1, stages.Count);
        playerText.text = "玩家\n生命 " + playerHp + "/" + PlayerMaxHp + "   格挡 " + playerBlock;
        enemyText.text = "阶段 " + stageDisplay + "/" + stages.Count + "  " + enemy.Name + "\n生命 " + enemyHp + "/" + enemyMaxHp + "   格挡 " + enemyBlock + "   意图 " + enemyIntent + " 点伤害";
        enemyHandText.text = "敌人手牌：" + EnemyHandLabels();
        enemyArtImage.sprite = LoadEnemySprite(enemy);
        enemyArtImage.color = enemyArtImage.sprite == null ? new Color(1f, 1f, 1f, 0f) : Color.white;
        statusText.text = "能量 " + energy + "/" + MaxEnergy + "\n第 " + turn + " 回合";
        drawPileText.text = "牌库\n" + deck.Count;
        discardPileText.text = "弃牌\n" + discard.Count;
        endTurnButton.GetComponentInChildren<Text>().text = gameOver ? "重新开始" : "结束回合";

        foreach (Transform child in handRoot)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < hand.Count; i++)
        {
            int cardIndex = i;
            CardDef card = hand[i];
            Button button = CreateCardButton(handRoot, card);
            RectTransform rect = button.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(170f, 226f);
            button.interactable = !gameOver && energy >= card.Cost;
            button.onClick.AddListener(() => PlayCard(cardIndex));
        }

        logText.text = string.Join("\n", logLines.ToArray());
    }

    private string CardLabel(CardDef card)
    {
        return card.Name + "\n费用 " + card.Cost + "\n\n" + card.Description;
    }

    private Sprite LoadCardSprite(CardDef card)
    {
        if (string.IsNullOrEmpty(card.ArtPath))
        {
            return null;
        }

        if (cardSprites.TryGetValue(card.ArtPath, out Sprite cachedSprite))
        {
            return cachedSprite;
        }

        Texture2D texture = Resources.Load<Texture2D>(card.ArtPath);
        if (texture == null)
        {
            Debug.LogWarning("未找到卡牌素材: Resources/" + card.ArtPath);
            cardSprites[card.ArtPath] = null;
            return null;
        }
        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        cardSprites[card.ArtPath] = sprite;
        return sprite;
    }

    private Sprite LoadEnemySprite(EnemyDef enemy)
    {
        if (string.IsNullOrEmpty(enemy.ArtPath))
        {
            return null;
        }

        if (enemySprites.TryGetValue(enemy.ArtPath, out Sprite cachedSprite))
        {
            return cachedSprite;
        }

        Texture2D texture = Resources.Load<Texture2D>(enemy.ArtPath);
        if (texture == null)
        {
            Debug.LogWarning("未找到敌人素材: Resources/" + enemy.ArtPath);
            enemySprites[enemy.ArtPath] = null;
            return null;
        }

        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        enemySprites[enemy.ArtPath] = sprite;
        return sprite;
    }

    private string EnemyHandNames()
    {
        if (enemyHand.Count == 0)
        {
            return "无";
        }

        List<string> names = new List<string>();
        for (int i = 0; i < enemyHand.Count; i++)
        {
            names.Add("「" + enemyHand[i].Name + "」");
        }

        return string.Join("、", names.ToArray());
    }

    private string EnemyHandLabels()
    {
        if (enemyHand.Count == 0)
        {
            return "无";
        }

        List<string> labels = new List<string>();
        for (int i = 0; i < enemyHand.Count; i++)
        {
            EnemyCardDef card = enemyHand[i];
            labels.Add(card.Name + "(" + card.Description + ")");
        }

        return string.Join("  |  ", labels.ToArray());
    }

    private string BuildCardEffectText(CardDef card)
    {
        List<string> parts = new List<string>();
        if (card.Damage > 0)
        {
            parts.Add("伤害 +" + card.Damage);
        }
        if (card.Block > 0)
        {
            parts.Add("格挡 +" + card.Block);
        }
        if (card.Heal > 0)
        {
            parts.Add("生命 +" + card.Heal);
        }

        return "打出「" + card.Name + "」\n" + string.Join("   ", parts.ToArray());
    }

    private void ShowCardEffect(string message, Color color)
    {
        if (effectText == null)
        {
            return;
        }

        if (effectRoutine != null)
        {
            StopCoroutine(effectRoutine);
        }

        effectRoutine = StartCoroutine(PlayEffectRoutine(message, color));
    }

    private IEnumerator PlayEffectRoutine(string message, Color color)
    {
        effectText.text = message;
        effectText.color = Color.Lerp(color, Color.white, 0.25f);
        effectText.canvasRenderer.SetAlpha(0f);
        effectText.rectTransform.anchoredPosition = new Vector2(-42f, -42f);
        effectText.rectTransform.localScale = Vector3.one * 0.82f;

        float timer = 0f;
        while (timer < 0.22f)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / 0.22f);
            effectText.canvasRenderer.SetAlpha(t);
            effectText.rectTransform.localScale = Vector3.one * Mathf.Lerp(0.82f, 1.12f, t);
            yield return null;
        }

        timer = 0f;
        while (timer < 0.55f)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / 0.55f);
            effectText.canvasRenderer.SetAlpha(1f - t);
            effectText.rectTransform.anchoredPosition = new Vector2(-42f, Mathf.Lerp(-42f, -96f, t));
            effectText.rectTransform.localScale = Vector3.one * Mathf.Lerp(1.12f, 1.22f, t);
            yield return null;
        }

        effectText.canvasRenderer.SetAlpha(0f);
        effectRoutine = null;
    }

    private Button CreateButton(Transform parent, string name, string label, Color color)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        Image image = buttonObject.GetComponent<Image>();
        image.color = color;
        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = Color.Lerp(color, Color.white, 0.18f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.18f);
        colors.disabledColor = new Color(0.28f, 0.28f, 0.30f, 0.65f);
        button.colors = colors;

        Text text = CreateText(buttonObject.transform, "Label", label, 18, FontStyle.Bold, Color.white);
        SetRect(text.rectTransform, AnchorStretch(), AnchorStretch(), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        text.rectTransform.offsetMin = new Vector2(8f, 8f);
        text.rectTransform.offsetMax = new Vector2(-8f, -8f);
        return button;
    }

    private Button CreateCardButton(Transform parent, CardDef card)
    {
        Sprite cardSprite = LoadCardSprite(card);
        if (cardSprite == null)
        {
            return CreateButton(parent, card.Name, CardLabel(card), card.Tint);
        }

        GameObject buttonObject = new GameObject(card.Name, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        Image image = buttonObject.GetComponent<Image>();
        image.sprite = cardSprite;
        image.color = Color.white;
        image.preserveAspect = true;

        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 1f, 1f, 0.92f);
        colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
        colors.disabledColor = new Color(0.42f, 0.42f, 0.42f, 0.55f);
        button.colors = colors;

        
        return button;
    }

    

    private Image CreateImage(Transform parent, string name, Color color)
    {
        GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(parent, false);
        Image image = imageObject.GetComponent<Image>();
        image.color = color;
        image.preserveAspect = true;
        image.raycastTarget = false;
        return image;
    }

    private Image CreatePanel(Transform parent, string name, RectTransform source, Color color)
    {
        Image image = CreatePanel(parent, name, AnchorStretch(), color);
        RectTransform rect = image.rectTransform;
        rect.anchorMin = source.anchorMin;
        rect.anchorMax = source.anchorMax;
        rect.pivot = source.pivot;
        rect.anchoredPosition = source.anchoredPosition;
        rect.sizeDelta = source.sizeDelta;
        return image;
    }

    private Image CreatePanel(Transform parent, string name, Vector2 anchor, Color color)
    {
        GameObject panelObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        panelObject.transform.SetParent(parent, false);
        Image image = panelObject.GetComponent<Image>();
        image.color = color;
        RectTransform rect = image.rectTransform;
        if (anchor == AnchorStretch())
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        return image;
    }

    private Text CreateText(Transform parent, string name, string value, int size, FontStyle style, Color color)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(parent, false);
        Text text = textObject.GetComponent<Text>();
        text.text = value;
        text.font = uiFont;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    private static Vector2 AnchorStretch()
    {
        return new Vector2(-1f, -1f);
    }

    private static RectTransform RectFrom(RectTransform source)
    {
        return source;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 size)
    {
        if (anchorMin == AnchorStretch() && anchorMax == AnchorStretch())
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return;
        }

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
    }

    private void AddLog(string message)
    {
        logLines.Enqueue(message);
        while (logLines.Count > 7)
        {
            logLines.Dequeue();
        }
    }

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
