using System;
using UnityEngine;
using UnityEngine.UI;

public class LegacyEchoView : MonoBehaviour
{
    public Text SourceTravelerText;
    public Text RegionHintText;
    public Text CauseText;
    public Text RewardText;
    public Button ClaimImmediateButton;
    public Button ResearchInVillageButton;

    private LegacyEcho echo;
    private Action<LegacyEcho> claimImmediate;
    private Action<LegacyEcho> researchInVillage;

    public void Bind(LegacyEcho legacyEcho, Action<LegacyEcho> onClaimImmediate, Action<LegacyEcho> onResearchInVillage)
    {
        echo = legacyEcho;
        claimImmediate = onClaimImmediate;
        researchInVillage = onResearchInVillage;

        BindButtons();
        Render(echo);
    }

    public void Render(LegacyEcho legacyEcho)
    {
        echo = legacyEcho;

        SetText(SourceTravelerText, "来源旅行者：" + (echo == null ? "未知" : "#" + echo.SourceTravelerId));
        SetText(RegionHintText, "区域线索：" + (echo == null ? "未知" : EmptyToUnknown(echo.RegionHint)));
        SetText(CauseText, "死亡原因：" + (echo == null ? "未知" : EmptyToUnknown(echo.Cause)));
        SetText(RewardText, BuildRewardText(echo));

        bool canResolve = echo != null && !echo.IsRecovered;
        SetButtonInteractable(ClaimImmediateButton, canResolve);
        SetButtonInteractable(ResearchInVillageButton, canResolve);
    }

    private void BindButtons()
    {
        if (ClaimImmediateButton != null)
        {
            ClaimImmediateButton.onClick.RemoveAllListeners();
            ClaimImmediateButton.onClick.AddListener(delegate
            {
                if (claimImmediate != null)
                {
                    claimImmediate(echo);
                }
            });
        }

        if (ResearchInVillageButton != null)
        {
            ResearchInVillageButton.onClick.RemoveAllListeners();
            ResearchInVillageButton.onClick.AddListener(delegate
            {
                if (researchInVillage != null)
                {
                    researchInVillage(echo);
                }
            });
        }
    }

    private static string BuildRewardText(LegacyEcho echo)
    {
        if (echo == null)
        {
            return "奖励说明：未知";
        }

        return "奖励说明：" + EmptyToUnknown(echo.RewardPayload)
            + "\n立刻吸收：" + EmptyToUnknown(echo.ImmediateClaimEffect)
            + "\n带回村庄研究：" + EmptyToUnknown(echo.ResearchEffect);
    }

    private static string EmptyToUnknown(string value)
    {
        return string.IsNullOrEmpty(value) ? "未知" : value;
    }

    private static void SetButtonInteractable(Button button, bool interactable)
    {
        if (button != null)
        {
            button.interactable = interactable;
        }
    }

    private static void SetText(Text text, string value)
    {
        if (text != null)
        {
            text.text = value;
        }
    }
}
