public class TableAbility
{
    public string AbilityId;
    public string DisplayName;
    public bool IsActiveSkill;

    public TableAbility(string abilityId, string displayName, bool isActiveSkill)
    {
        AbilityId = abilityId;
        DisplayName = displayName;
        IsActiveSkill = isActiveSkill;
    }
}
