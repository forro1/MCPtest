using System.Collections.Generic;

public enum TableUnderstandingStage
{
    Silent,
    Responding,
    Cooperating,
    Awakened
}

public class TableProgress
{
    public TableUnderstandingStage UnderstandingStage = TableUnderstandingStage.Silent;
    public readonly List<string> UnlockedPassiveIds = new List<string>();
    public readonly List<string> UnlockedActiveSkillIds = new List<string>();
    public readonly List<string> ResonanceRules = new List<string>();
    public readonly List<string> AwakeningTags = new List<string>();

    public void UnlockPassive(string passiveId)
    {
        AddUnique(UnlockedPassiveIds, passiveId);
    }

    public void UnlockActiveSkill(string skillId)
    {
        AddUnique(UnlockedActiveSkillIds, skillId);
    }

    public bool HasPassive(string passiveId)
    {
        return UnlockedPassiveIds.Contains(passiveId);
    }

    public bool HasActiveSkill(string skillId)
    {
        return UnlockedActiveSkillIds.Contains(skillId);
    }

    private static void AddUnique(List<string> list, string value)
    {
        if (!string.IsNullOrEmpty(value) && !list.Contains(value))
        {
            list.Add(value);
        }
    }
}
