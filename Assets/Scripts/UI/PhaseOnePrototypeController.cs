using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PhaseOnePrototypeController : MonoBehaviour
{
    private GameFlowController flow;
    private Canvas canvas;
    private Font uiFont;
    private VillageView villageView;
    private ExplorationMapView mapView;
    private LegacyEchoView echoView;
    private RunSummaryView summaryView;
    private GameObject villagePanel;
    private GameObject mapPanel;
    private GameObject battlePanel;
    private GameObject echoPanel;
    private GameObject summaryPanel;
    private Text phaseText;
    private Text nodeResultText;
    private Text battlePlayerText;
    private Text battleEnemyText;
    private Text battleStatusText;
    private Text battleLogText;
    private Transform battleHandRoot;
    private Button battleCardTemplate;
    private Button endTurnButton;
    private Button concedeButton;
    private LegacyEcho lastGeneratedEcho;
    private MapNodeIntel currentNode;
    private BattleState manualBattleState;
    private TurnController manualBattleController;

    public void Initialize()
    {
        flow = new GameFlowController(new VillageState());
        uiFont = Font.CreateDynamicFontFromOSFont(new[] { "Microsoft YaHei", "SimHei", "Arial" }, 18);
        EnsureEventSystem();
        BuildUi();
        ShowVillage();
    }

    private void BuildUi()
    {
        GameObject canvasObject = new GameObject("Phase 1 Prototype Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        CreatePanel(canvasObject.transform, "Background", Stretch(), new Color(0.09f, 0.11f, 0.14f));
        Text title = CreateText(canvasObject.transform, "Title", "Phase 1 代际循环原型", 34, FontStyle.Bold, new Color(0.96f, 0.86f, 0.62f));
        SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -52f), new Vector2(900f, 70f));
        phaseText = CreateText(canvasObject.transform, "Phase Text", string.Empty, 18, FontStyle.Bold, new Color(0.78f, 0.86f, 0.96f));
        SetRect(phaseText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -112f), new Vector2(900f, 46f));

        villagePanel = CreateContentPanel(canvasObject.transform, "Village Panel");
        mapPanel = CreateContentPanel(canvasObject.transform, "Map Panel");
        battlePanel = CreateContentPanel(canvasObject.transform, "Battle Panel");
        echoPanel = CreateContentPanel(canvasObject.transform, "Echo Panel");
        summaryPanel = CreateContentPanel(canvasObject.transform, "Summary Panel");

        BuildVillagePanel();
        BuildMapPanel();
        BuildBattlePanel();
        BuildEchoPanel();
        BuildSummaryPanel();
    }

    private void BuildVillagePanel()
    {
        villageView = villagePanel.AddComponent<VillageView>();
        villageView.TrainingLevelText = CreatePanelText(villagePanel.transform, "Training", new Vector2(-430f, 200f), new Vector2(520f, 54f));
        villageView.TravelerCountText = CreatePanelText(villagePanel.transform, "Traveler Count", new Vector2(-430f, 132f), new Vector2(520f, 54f));
        villageView.UnrecoveredEchoCountText = CreatePanelText(villagePanel.transform, "Echo Count", new Vector2(-430f, 64f), new Vector2(520f, 54f));
        villageView.CurrentTravelerText = CreatePanelText(villagePanel.transform, "Current Traveler", new Vector2(260f, 116f), new Vector2(620f, 180f));
        villageView.StartNewTravelerButton = CreateButton(villagePanel.transform, "Start Traveler", "开始新旅行者", new Vector2(-180f, -190f), new Vector2(260f, 66f), new Color(0.28f, 0.58f, 0.42f));
        villageView.ViewMapButton = CreateButton(villagePanel.transform, "View Map", "查看地图", new Vector2(180f, -190f), new Vector2(260f, 66f), new Color(0.26f, 0.48f, 0.72f));
    }

    private void BuildMapPanel()
    {
        mapView = mapPanel.AddComponent<ExplorationMapView>();
        mapView.CurrentNodeText = CreatePanelText(mapPanel.transform, "Current Node", new Vector2(-440f, 238f), new Vector2(560f, 48f));
        mapView.ReachableIntelText = CreatePanelText(mapPanel.transform, "Reachable Intel", new Vector2(-260f, 70f), new Vector2(920f, 260f));
        mapView.NodeButtonRoot = CreateButtonRoot(mapPanel.transform, "Node Buttons", new Vector2(430f, 50f), new Vector2(360f, 300f));
        mapView.NodeButtonTemplate = CreateButton(mapView.NodeButtonRoot, "Node Button Template", "进入节点", new Vector2(0f, 108f), new Vector2(300f, 58f), new Color(0.34f, 0.43f, 0.70f));
        nodeResultText = CreatePanelText(mapPanel.transform, "Node Result", new Vector2(0f, -142f), new Vector2(1040f, 82f));
    }

    private void BuildBattlePanel()
    {
        battlePlayerText = CreatePanelText(battlePanel.transform, "Battle Player", new Vector2(-430f, 218f), new Vector2(520f, 58f));
        battleEnemyText = CreatePanelText(battlePanel.transform, "Battle Enemy", new Vector2(300f, 218f), new Vector2(640f, 58f));
        battleStatusText = CreatePanelText(battlePanel.transform, "Battle Status", new Vector2(-430f, 140f), new Vector2(520f, 58f));
        battleLogText = CreatePanelText(battlePanel.transform, "Battle Log", new Vector2(310f, 38f), new Vector2(640f, 250f));
        battleHandRoot = CreateButtonRoot(battlePanel.transform, "Battle Hand", new Vector2(-260f, -90f), new Vector2(620f, 220f));
        battleCardTemplate = CreateButton(battleHandRoot, "Battle Card Template", "卡牌", new Vector2(0f, 80f), new Vector2(300f, 56f), new Color(0.48f, 0.40f, 0.68f));
        endTurnButton = CreateButton(battlePanel.transform, "End Manual Turn", "结束回合", new Vector2(240f, -222f), new Vector2(240f, 64f), new Color(0.82f, 0.50f, 0.24f));
        concedeButton = CreateButton(battlePanel.transform, "Concede Battle", "放弃并死亡", new Vector2(520f, -222f), new Vector2(240f, 64f), new Color(0.68f, 0.28f, 0.28f));
        endTurnButton.onClick.AddListener(EndManualBattleTurn);
        concedeButton.onClick.AddListener(ConcedeManualBattle);
    }

    private void BuildEchoPanel()
    {
        echoView = echoPanel.AddComponent<LegacyEchoView>();
        echoView.SourceTravelerText = CreatePanelText(echoPanel.transform, "Echo Source", new Vector2(-350f, 176f), new Vector2(680f, 54f));
        echoView.RegionHintText = CreatePanelText(echoPanel.transform, "Echo Region", new Vector2(-350f, 104f), new Vector2(680f, 54f));
        echoView.CauseText = CreatePanelText(echoPanel.transform, "Echo Cause", new Vector2(-350f, 32f), new Vector2(680f, 54f));
        echoView.RewardText = CreatePanelText(echoPanel.transform, "Echo Reward", new Vector2(310f, 82f), new Vector2(640f, 190f));
        echoView.ClaimImmediateButton = CreateButton(echoPanel.transform, "Claim Immediate", "立刻吸收", new Vector2(-190f, -210f), new Vector2(260f, 66f), new Color(0.58f, 0.42f, 0.74f));
        echoView.ResearchInVillageButton = CreateButton(echoPanel.transform, "Research Echo", "带回村庄研究", new Vector2(190f, -210f), new Vector2(300f, 66f), new Color(0.32f, 0.56f, 0.62f));
    }

    private void BuildSummaryPanel()
    {
        summaryView = summaryPanel.AddComponent<RunSummaryView>();
        summaryView.DeathReasonText = CreatePanelText(summaryPanel.transform, "Death Reason", new Vector2(-330f, 174f), new Vector2(720f, 58f));
        summaryView.DeathRegionText = CreatePanelText(summaryPanel.transform, "Death Region", new Vector2(-330f, 102f), new Vector2(720f, 58f));
        summaryView.LegacyEchoText = CreatePanelText(summaryPanel.transform, "Generated Echo", new Vector2(300f, 112f), new Vector2(620f, 170f));
        summaryView.LongTermChangeText = CreatePanelText(summaryPanel.transform, "Long Term", new Vector2(0f, -64f), new Vector2(980f, 84f));
        summaryView.ReturnVillageButton = CreateButton(summaryPanel.transform, "Return Village", "返回村庄", new Vector2(-190f, -220f), new Vector2(260f, 66f), new Color(0.28f, 0.50f, 0.64f));
        summaryView.StartNextTravelerButton = CreateButton(summaryPanel.transform, "Start Next", "开始下一任", new Vector2(190f, -220f), new Vector2(260f, 66f), new Color(0.28f, 0.58f, 0.42f));
    }

    private void ShowVillage()
    {
        SetActive(villagePanel);
        phaseText.text = "村庄：查看长期状态，开始或继续代际循环";
        villageView.Bind(flow.Village, flow.CurrentTraveler, delegate
        {
            flow.StartNewTraveler();
            ShowMap();
        }, ShowMap);
    }

    private void ShowMap()
    {
        if (flow.CurrentTraveler == null)
        {
            ShowVillage();
            return;
        }

        currentNode = null;
        SetActive(mapPanel);
        phaseText.text = "地图：情报并不完全可靠，选择一个可达节点";
        nodeResultText.text = "选择节点后会显示实际结果。";
        mapView.Bind(flow.Exploration, OnNodeSelected);
    }

    private void OnNodeSelected(string nodeId)
    {
        ExplorationNodeResult result = flow.EnterNode(nodeId);
        currentNode = result.Node;
        nodeResultText.text = result.IntelComparisonText + "\n来源：" + currentNode.IntelSource;

        if (result.ResultType == MapNodeType.Battle)
        {
            ShowBattle();
            return;
        }

        if (result.ResultType == MapNodeType.LegacyEcho)
        {
            ShowEcho();
        }
        else if (result.ResultType == MapNodeType.Rest)
        {
            flow.CurrentTraveler.CurrentHp = Mathf.Min(flow.CurrentTraveler.MaxHp, flow.CurrentTraveler.CurrentHp + 8);
            nodeResultText.text += "\n营地恢复了 8 点生命。";
            mapView.Render(flow.Exploration);
        }
        else if (result.ResultType == MapNodeType.RegionEnd)
        {
            nodeResultText.text += "\n本次最小探索抵达阶段终点。";
        }
        else
        {
            nodeResultText.text += "\n事件留下了新的判断空间。";
            mapView.Render(flow.Exploration);
        }
    }

    private void ShowBattle()
    {
        SetActive(battlePanel);
        phaseText.text = "战斗：复用能量、抽牌、出牌、敌人行动与胜负结算";
        StartManualBattle();
        RefreshManualBattle();
    }

    private void StartManualBattle()
    {
        BattleRunRequest request = BattleRunRequestFactory.CreatePhaseOneRequest(flow.CurrentTraveler, currentNode);

        manualBattleState = new BattleState();
        manualBattleController = new TurnController(manualBattleState, Random.Range);
        manualBattleController.NewGame(
            request.PlayerMaxHp,
            request.MaxEnergy,
            request.HandSize,
            request.EnemyHandSize,
            request.StartingDeck,
            request.Stages);
        manualBattleState.PlayerHp = Mathf.Clamp(request.PlayerCurrentHp, 0, request.PlayerMaxHp);
        ApplyManualTableAbilities(request);
    }

    private void ApplyManualTableAbilities(BattleRunRequest request)
    {
        if (request.TableAbilityIds.Contains("memory_spark"))
        {
            manualBattleState.PlayerBlock += 2;
        }

        if (request.TableAbilityIds.Contains("echo_call"))
        {
            manualBattleState.Energy += 1;
        }
    }

    private void RefreshManualBattle()
    {
        if (manualBattleState == null)
        {
            return;
        }

        battlePlayerText.text = "旅行者 #" + flow.CurrentTraveler.TravelerId + "  生命 "
            + manualBattleState.PlayerHp + "/" + manualBattleState.PlayerMaxHp
            + "  格挡 " + manualBattleState.PlayerBlock;
        string enemyName = "已通关";
        if (manualBattleState.StageIndex >= 0 && manualBattleState.StageIndex < manualBattleState.Stages.Count && manualBattleState.Stages[manualBattleState.StageIndex].Enemy != null)
        {
            enemyName = manualBattleState.Stages[manualBattleState.StageIndex].Enemy.Name;
        }

        battleEnemyText.text = "敌人 " + enemyName + "  生命 "
            + manualBattleState.EnemyHp + "/" + manualBattleState.EnemyMaxHp
            + "  格挡 " + manualBattleState.EnemyBlock
            + "  意图 " + manualBattleState.EnemyIntent;
        battleStatusText.text = "能量 " + manualBattleState.Energy + "/" + manualBattleState.MaxEnergy
            + "  回合 " + manualBattleState.Turn;
        battleLogText.text = string.Join("\n", manualBattleState.LogLines.ToArray());
        RenderBattleHand();
        FinishManualBattleIfGameOver();
    }

    private void RenderBattleHand()
    {
        for (int i = battleHandRoot.childCount - 1; i >= 0; i--)
        {
            Transform child = battleHandRoot.GetChild(i);
            if (child != battleCardTemplate.transform)
            {
                Destroy(child.gameObject);
            }
        }

        battleCardTemplate.gameObject.SetActive(false);
        for (int i = 0; i < manualBattleState.PlayerDeck.Hand.Count; i++)
        {
            int cardIndex = i;
            CardData card = manualBattleState.PlayerDeck.Hand[i];
            Button button = Instantiate(battleCardTemplate, battleHandRoot);
            button.name = "Battle Card " + i;
            button.gameObject.SetActive(true);
            Text label = button.GetComponentInChildren<Text>();
            label.text = card.Name + "  费 " + card.Cost + "\n" + card.Description;
            button.interactable = !manualBattleState.GameOver && manualBattleState.Energy >= card.Cost;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate
            {
                manualBattleController.PlayCard(cardIndex);
                RefreshManualBattle();
            });
        }
    }

    private void EndManualBattleTurn()
    {
        if (manualBattleController == null || manualBattleState == null || manualBattleState.GameOver)
        {
            return;
        }

        manualBattleController.EndTurn();
        RefreshManualBattle();
    }

    private void ConcedeManualBattle()
    {
        ApplyManualBattleResult(new BattleRunResult(false, true, 0, "Defeated in battle"));
    }

    private void FinishManualBattleIfGameOver()
    {
        if (manualBattleState == null || !manualBattleState.GameOver)
        {
            return;
        }

        bool victory = manualBattleState.StageIndex >= manualBattleState.Stages.Count && manualBattleState.PlayerHp > 0;
        bool defeat = manualBattleState.PlayerHp <= 0 || !victory;
        ApplyManualBattleResult(new BattleRunResult(victory, defeat, manualBattleState.PlayerHp, defeat ? "Defeated in battle" : string.Empty));
    }

    private void ApplyManualBattleResult(BattleRunResult result)
    {
        flow.ApplyBattleResult(result, currentNode == null ? "mist-woods" : currentNode.RegionId);
        if (result.IsDefeat)
        {
            lastGeneratedEcho = flow.Village.LegacyEchoes.Count == 0 ? null : flow.Village.LegacyEchoes[flow.Village.LegacyEchoes.Count - 1];
            ShowSummary();
            return;
        }

        ShowMap();
    }

    private void ShowEcho()
    {
        LegacyEcho echo = FirstVisibleEcho();
        if (echo == null)
        {
            nodeResultText.text += "\n这里没有可处理的前任回声。";
            return;
        }

        SetActive(echoPanel);
        phaseText.text = "前任回声：选择当前局强化，或带回村庄研究";
        echoView.Bind(echo, delegate
        {
            flow.ResolveVisibleEcho(true);
            ShowMap();
        }, delegate
        {
            flow.ResolveVisibleEcho(false);
            ShowVillage();
        });
    }

    private void ShowSummary()
    {
        SetActive(summaryPanel);
        phaseText.text = "死亡结算：前任行动已改变村庄和下一任目标";
        summaryView.Bind(flow.CurrentTraveler, lastGeneratedEcho, flow.Village, ShowVillage, delegate
        {
            flow.StartNewTraveler();
            ShowMap();
        });
    }

    private LegacyEcho FirstVisibleEcho()
    {
        if (flow.CurrentTraveler == null)
        {
            return null;
        }

        for (int i = 0; i < flow.CurrentTraveler.VisibleLegacyEchoes.Count; i++)
        {
            LegacyEcho echo = flow.CurrentTraveler.VisibleLegacyEchoes[i];
            if (echo != null && !echo.IsRecovered)
            {
                return echo;
            }
        }

        return null;
    }

    private void SetActive(GameObject activePanel)
    {
        villagePanel.SetActive(activePanel == villagePanel);
        mapPanel.SetActive(activePanel == mapPanel);
        battlePanel.SetActive(activePanel == battlePanel);
        echoPanel.SetActive(activePanel == echoPanel);
        summaryPanel.SetActive(activePanel == summaryPanel);
    }

    private GameObject CreateContentPanel(Transform parent, string name)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);
        Image image = panel.GetComponent<Image>();
        image.color = new Color(0.13f, 0.15f, 0.19f, 0.96f);
        SetRect(panel.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -28f), new Vector2(1220f, 720f));
        return panel;
    }

    private Transform CreateButtonRoot(Transform parent, string name, Vector2 anchoredPosition, Vector2 size)
    {
        GameObject root = new GameObject(name, typeof(RectTransform), typeof(VerticalLayoutGroup));
        root.transform.SetParent(parent, false);
        SetRect(root.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), anchoredPosition, size);
        VerticalLayoutGroup layout = root.GetComponent<VerticalLayoutGroup>();
        layout.spacing = 12f;
        layout.childControlHeight = false;
        layout.childControlWidth = false;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = false;
        layout.childAlignment = TextAnchor.MiddleCenter;
        return root.transform;
    }

    private Text CreatePanelText(Transform parent, string name, Vector2 anchoredPosition, Vector2 size)
    {
        Text text = CreateText(parent, name, string.Empty, 20, FontStyle.Bold, new Color(0.92f, 0.94f, 0.96f));
        text.alignment = TextAnchor.MiddleLeft;
        SetRect(text.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), anchoredPosition, size);
        return text;
    }

    private Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPosition, Vector2 size, Color color)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        Image image = buttonObject.GetComponent<Image>();
        image.color = color;
        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = Color.Lerp(color, Color.white, 0.16f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.16f);
        colors.disabledColor = new Color(0.28f, 0.28f, 0.30f, 0.55f);
        button.colors = colors;
        SetRect(button.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), anchoredPosition, size);

        Text text = CreateText(buttonObject.transform, "Label", label, 18, FontStyle.Bold, Color.white);
        SetRect(text.rectTransform, Stretch(), Stretch(), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        text.rectTransform.offsetMin = new Vector2(8f, 8f);
        text.rectTransform.offsetMax = new Vector2(-8f, -8f);
        return button;
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

    private Image CreatePanel(Transform parent, string name, Vector2 anchor, Color color)
    {
        GameObject panelObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        panelObject.transform.SetParent(parent, false);
        Image image = panelObject.GetComponent<Image>();
        image.color = color;
        RectTransform rect = image.rectTransform;
        if (anchor == Stretch())
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        return image;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 size)
    {
        if (anchorMin == Stretch() && anchorMax == Stretch())
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

    private static Vector2 Stretch()
    {
        return new Vector2(-1f, -1f);
    }

    private static void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem));
        System.Type inputSystemModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
        if (inputSystemModuleType != null)
        {
            eventSystem.AddComponent(inputSystemModuleType);
        }
        else
        {
            eventSystem.AddComponent<StandaloneInputModule>();
        }
    }
}
