public static class LegacyEchoResolver
{
    public const int MaxTrainingLevel = 3;

    public static bool ResolveImmediate(LegacyEcho echo, TravelerRun traveler, VillageState village)
    {
        if (!CanResolve(echo))
        {
            return false;
        }

        if (traveler != null)
        {
            traveler.IncreaseMaxHp(5, true);
            traveler.FoundLegacyEchoIds.Add(echo.EchoId);
        }

        echo.IsRecovered = true;
        return true;
    }

    public static bool ResolveResearch(LegacyEcho echo, TravelerRun traveler, VillageState village)
    {
        if (!CanResolve(echo))
        {
            return false;
        }

        if (village != null)
        {
            ApplyResearchEffect(echo, village);
        }

        if (traveler != null)
        {
            traveler.FoundLegacyEchoIds.Add(echo.EchoId);
        }

        echo.IsRecovered = true;
        return true;
    }

    private static bool CanResolve(LegacyEcho echo)
    {
        return echo != null && !echo.IsRecovered;
    }

    private static void ApplyResearchEffect(LegacyEcho echo, VillageState village)
    {
        string effect = string.IsNullOrEmpty(echo.ResearchEffect) ? "training_level" : echo.ResearchEffect;
        if (effect.StartsWith("unlock_card:"))
        {
            string cardId = effect.Substring("unlock_card:".Length);
            if (!string.IsNullOrEmpty(cardId) && !village.UnlockedCardIds.Contains(cardId))
            {
                village.UnlockedCardIds.Add(cardId);
            }
            return;
        }

        if (effect == "training_level")
        {
            village.TrainingLevel = System.Math.Min(MaxTrainingLevel, village.TrainingLevel + 1);
        }
    }
}
