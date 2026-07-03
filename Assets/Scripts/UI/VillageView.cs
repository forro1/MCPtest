using System;
using UnityEngine;
using UnityEngine.UI;

public class VillageView : MonoBehaviour
{
    public Text TrainingLevelText;
    public Text TravelerCountText;
    public Text UnrecoveredEchoCountText;
    public Text CurrentTravelerText;
    public Button StartNewTravelerButton;
    public Button ViewMapButton;

    private VillageState village;
    private TravelerRun currentTraveler;
    private Action startNewTraveler;
    private Action viewMap;

    public void Bind(VillageState villageState, TravelerRun traveler, Action onStartNewTraveler, Action onViewMap)
    {
        village = villageState;
        currentTraveler = traveler;
        startNewTraveler = onStartNewTraveler;
        viewMap = onViewMap;

        BindButtons();
        Render(village, currentTraveler);
    }

    public void Bind(GameFlowController flow, Action onViewMap)
    {
        if (flow == null)
        {
            Bind(null, null, null, onViewMap);
            return;
        }

        Bind(flow.Village, flow.CurrentTraveler, delegate
        {
            flow.StartNewTraveler();
            Render(flow.Village, flow.CurrentTraveler);
        }, onViewMap);
    }

    public void Render(VillageState villageState, TravelerRun traveler)
    {
        village = villageState;
        currentTraveler = traveler;

        SetText(TrainingLevelText, "训练等级：" + (village == null ? 0 : village.TrainingLevel));
        SetText(TravelerCountText, "旅行者数量：" + (village == null ? 0 : village.TravelerRecords.Count));
        SetText(UnrecoveredEchoCountText, "未找回回声：" + CountUnrecoveredEchoes(village));
        SetText(CurrentTravelerText, BuildTravelerSummary(traveler));
        if (ViewMapButton != null)
        {
            ViewMapButton.interactable = traveler != null;
        }
    }

    private void BindButtons()
    {
        if (StartNewTravelerButton != null)
        {
            StartNewTravelerButton.onClick.RemoveAllListeners();
            StartNewTravelerButton.onClick.AddListener(delegate
            {
                if (startNewTraveler != null)
                {
                    startNewTraveler();
                }
            });
        }

        if (ViewMapButton != null)
        {
            ViewMapButton.onClick.RemoveAllListeners();
            ViewMapButton.onClick.AddListener(delegate
            {
                if (viewMap != null)
                {
                    viewMap();
                }
            });
        }
    }

    private static int CountUnrecoveredEchoes(VillageState villageState)
    {
        if (villageState == null)
        {
            return 0;
        }

        int count = 0;
        for (int i = 0; i < villageState.LegacyEchoes.Count; i++)
        {
            LegacyEcho echo = villageState.LegacyEchoes[i];
            if (echo != null && !echo.IsRecovered)
            {
                count++;
            }
        }

        return count;
    }

    private static string BuildTravelerSummary(TravelerRun traveler)
    {
        if (traveler == null)
        {
            return "当前旅行者：无";
        }

        return "当前旅行者：#" + traveler.TravelerId
            + "\n生命：" + traveler.CurrentHp + "/" + traveler.MaxHp
            + "\n牌组：" + traveler.DeckCardIds.Count
            + "  遗物：" + traveler.RelicIds.Count
            + "  已访问：" + traveler.VisitedNodeIds.Count;
    }

    private static void SetText(Text text, string value)
    {
        if (text != null)
        {
            text.text = value;
        }
    }
}
