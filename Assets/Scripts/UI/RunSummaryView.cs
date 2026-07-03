using System;
using UnityEngine;
using UnityEngine.UI;

public class RunSummaryView : MonoBehaviour
{
    public Text DeathReasonText;
    public Text DeathRegionText;
    public Text LegacyEchoText;
    public Text LongTermChangeText;
    public Button ReturnVillageButton;
    public Button StartNextTravelerButton;

    private TravelerRun traveler;
    private LegacyEcho generatedEcho;
    private VillageState village;
    private Action returnVillage;
    private Action startNextTraveler;

    public void Bind(TravelerRun run, LegacyEcho echo, VillageState villageState, Action onReturnVillage, Action onStartNextTraveler)
    {
        traveler = run;
        generatedEcho = echo;
        village = villageState;
        returnVillage = onReturnVillage;
        startNextTraveler = onStartNextTraveler;

        BindButtons();
        Render(traveler, generatedEcho, village);
    }

    public void Render(TravelerRun run, LegacyEcho echo, VillageState villageState)
    {
        traveler = run;
        generatedEcho = echo;
        village = villageState;

        SetText(DeathReasonText, "死亡原因：" + (traveler == null ? "未知" : EmptyToUnknown(traveler.DeathReason)));
        SetText(DeathRegionText, "死亡区域：" + (traveler == null ? "未知" : EmptyToUnknown(traveler.DeathRegionId)));
        SetText(LegacyEchoText, BuildLegacyEchoText(generatedEcho));
        SetText(LongTermChangeText, BuildLongTermChangeText(village, generatedEcho));
    }

    public void Render(TravelerRun run, LegacyEcho echo, string longTermSummary)
    {
        traveler = run;
        generatedEcho = echo;

        SetText(DeathReasonText, "死亡原因：" + (traveler == null ? "未知" : EmptyToUnknown(traveler.DeathReason)));
        SetText(DeathRegionText, "死亡区域：" + (traveler == null ? "未知" : EmptyToUnknown(traveler.DeathRegionId)));
        SetText(LegacyEchoText, BuildLegacyEchoText(generatedEcho));
        SetText(LongTermChangeText, "长期变化：" + EmptyToUnknown(longTermSummary));
    }

    private void BindButtons()
    {
        if (ReturnVillageButton != null)
        {
            ReturnVillageButton.onClick.RemoveAllListeners();
            ReturnVillageButton.onClick.AddListener(delegate
            {
                if (returnVillage != null)
                {
                    returnVillage();
                }
            });
        }

        if (StartNextTravelerButton != null)
        {
            StartNextTravelerButton.onClick.RemoveAllListeners();
            StartNextTravelerButton.onClick.AddListener(delegate
            {
                if (startNextTraveler != null)
                {
                    startNextTraveler();
                }
            });
        }
    }

    private static string BuildLegacyEchoText(LegacyEcho echo)
    {
        if (echo == null)
        {
            return "生成回声：无";
        }

        return "生成回声：" + EmptyToUnknown(echo.EchoId)
            + "\n区域线索：" + EmptyToUnknown(echo.RegionHint)
            + "\n奖励：" + EmptyToUnknown(echo.RewardPayload);
    }

    private static string BuildLongTermChangeText(VillageState villageState, LegacyEcho echo)
    {
        if (villageState == null && echo == null)
        {
            return "长期变化：暂无";
        }

        int trainingLevel = villageState == null ? 0 : villageState.TrainingLevel;
        int travelerCount = villageState == null ? 0 : villageState.TravelerRecords.Count;
        int echoCount = villageState == null ? 0 : villageState.LegacyEchoes.Count;

        return "长期变化：训练等级 " + trainingLevel
            + "，旅行记录 " + travelerCount
            + "，村庄回声 " + echoCount;
    }

    private static string EmptyToUnknown(string value)
    {
        return string.IsNullOrEmpty(value) ? "未知" : value;
    }

    private static void SetText(Text text, string value)
    {
        if (text != null)
        {
            text.text = value;
        }
    }
}
