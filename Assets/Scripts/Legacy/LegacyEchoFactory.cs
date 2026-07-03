public static class LegacyEchoFactory
{
    public static LegacyEcho CreateFromDeath(TravelerRun traveler)
    {
        if (traveler == null)
        {
            return new LegacyEcho("echo-unknown", 0, "unknown", "Unknown", "Faint memory");
        }

        string region = string.IsNullOrEmpty(traveler.DeathRegionId) ? "unknown" : traveler.DeathRegionId;
        string cause = string.IsNullOrEmpty(traveler.DeathReason) ? "Unknown" : traveler.DeathReason;
        return new LegacyEcho(
            "echo-" + traveler.TravelerId,
            traveler.TravelerId,
            region,
            cause,
            "Memory from traveler " + traveler.TravelerId);
    }
}
