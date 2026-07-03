public static class LegacyEchoResolver
{
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
            village.TrainingLevel += 1;
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
}
