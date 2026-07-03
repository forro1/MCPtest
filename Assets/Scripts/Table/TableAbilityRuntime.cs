using System.Collections.Generic;

public class TableAbilityRuntime
{
    public readonly List<string> ActivePassiveIds = new List<string>();
    public readonly List<string> ActiveSkillIds = new List<string>();

    public static TableAbilityRuntime FromProgress(TableProgress progress)
    {
        TableAbilityRuntime runtime = new TableAbilityRuntime();
        if (progress == null)
        {
            return runtime;
        }

        if (progress.UnlockedPassiveIds.Count > 0)
        {
            runtime.ActivePassiveIds.Add(progress.UnlockedPassiveIds[0]);
        }

        if (progress.UnlockedActiveSkillIds.Count > 0)
        {
            runtime.ActiveSkillIds.Add(progress.UnlockedActiveSkillIds[0]);
        }

        return runtime;
    }
}
