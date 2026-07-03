using NUnit.Framework;

public class LegacyEchoTests
{
    [Test]
    public void FactoryCreatesUnrecoveredEchoFromDeadTraveler()
    {
        TravelerRun traveler = new TravelerRun(3, 50, new[] { "Strike", "Guard" });
        traveler.MarkDead("Overwhelmed", "mist-woods");

        LegacyEcho echo = LegacyEchoFactory.CreateFromDeath(traveler);

        Assert.IsFalse(echo.IsRecovered);
        Assert.AreEqual(3, echo.SourceTravelerId);
        Assert.AreEqual("mist-woods", echo.RegionHint);
        Assert.AreEqual("Overwhelmed", echo.Cause);
    }

    [Test]
    public void ImmediateClaimStrengthensCurrentTravelerAndRecoversEcho()
    {
        VillageState village = new VillageState();
        TravelerRun traveler = new TravelerRun(4, 40, new[] { "Strike" });
        LegacyEcho echo = new LegacyEcho("echo-4", 3, "mist-woods", "Overwhelmed", "Legacy vigor");

        LegacyEchoResolver.ResolveImmediate(echo, traveler, village);

        Assert.IsTrue(echo.IsRecovered);
        Assert.AreEqual(45, traveler.MaxHp);
        Assert.AreEqual(45, traveler.CurrentHp);
        Assert.AreEqual(0, village.TrainingLevel);
    }

    [Test]
    public void ResearchClaimImprovesVillageAndCannotRepeat()
    {
        VillageState village = new VillageState();
        TravelerRun traveler = new TravelerRun(4, 40, new[] { "Strike" });
        LegacyEcho echo = new LegacyEcho("echo-4", 3, "mist-woods", "Overwhelmed", "Legacy vigor");

        LegacyEchoResolver.ResolveResearch(echo, traveler, village);
        LegacyEchoResolver.ResolveResearch(echo, traveler, village);

        Assert.IsTrue(echo.IsRecovered);
        Assert.AreEqual(1, village.TrainingLevel);
        Assert.AreEqual(40, traveler.MaxHp);
    }
}
