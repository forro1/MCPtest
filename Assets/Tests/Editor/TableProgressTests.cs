using NUnit.Framework;

public class TableProgressTests
{
    [Test]
    public void NewTableProgressStartsSilent()
    {
        TableProgress progress = new TableProgress();

        Assert.AreEqual(TableUnderstandingStage.Silent, progress.UnderstandingStage);
        Assert.IsFalse(progress.HasPassive("memory_spark"));
        Assert.IsFalse(progress.HasActiveSkill("echo_call"));
    }

    [Test]
    public void TableProgressTracksUnlockedPassiveAndActiveSkill()
    {
        TableProgress progress = new TableProgress();

        progress.UnlockPassive("memory_spark");
        progress.UnlockActiveSkill("echo_call");

        Assert.IsTrue(progress.HasPassive("memory_spark"));
        Assert.IsTrue(progress.HasActiveSkill("echo_call"));
    }

    [Test]
    public void RuntimeLimitsPhaseOneActiveAbilities()
    {
        TableProgress progress = new TableProgress();
        progress.UnlockPassive("memory_spark");
        progress.UnlockActiveSkill("echo_call");
        progress.UnlockActiveSkill("time_nudge");

        TableAbilityRuntime runtime = TableAbilityRuntime.FromProgress(progress);

        Assert.AreEqual(1, runtime.ActivePassiveIds.Count);
        Assert.AreEqual(1, runtime.ActiveSkillIds.Count);
    }
}
