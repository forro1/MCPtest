using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class SimpleCardBattle2D
{
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
        playerText.text = "玩家\n生命 " + playerHp + "/" + playerMaxHp + "   格挡 " + playerBlock;
        enemyText.text = "阶段 " + stageDisplay + "/" + stages.Count + "  " + enemy.Name + "\n生命 " + enemyHp + "/" + enemyMaxHp + "   格挡 " + enemyBlock + "   意图 " + enemyIntent + " 点伤害";
        enemyHandText.text = "敌人手牌：" + EnemyHandLabels();
        enemyArtImage.sprite = LoadEnemySprite(enemy);
        enemyArtImage.color = enemyArtImage.sprite == null ? new Color(1f, 1f, 1f, 0f) : Color.white;
        statusText.text = "能量 " + energy + "/" + maxEnergy + "\n第 " + turn + " 回合";
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
}
