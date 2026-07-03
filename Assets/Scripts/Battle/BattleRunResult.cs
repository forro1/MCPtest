public class BattleRunResult
{
    public bool IsVictory;
    public bool IsDefeat;
    public int RemainingHp;
    public string DeathReason;

    public BattleRunResult(bool isVictory, bool isDefeat, int remainingHp, string deathReason)
    {
        IsVictory = isVictory;
        IsDefeat = isDefeat;
        RemainingHp = remainingHp;
        DeathReason = deathReason ?? string.Empty;
    }
}
