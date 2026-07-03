using System.Collections.Generic;

public class BattleRunRequest
{
    public int PlayerMaxHp;
    public int PlayerCurrentHp;
    public int MaxEnergy;
    public int HandSize;
    public int EnemyHandSize;
    public readonly List<DeckEntry> StartingDeck = new List<DeckEntry>();
    public readonly List<StageData> Stages = new List<StageData>();
    public readonly List<string> TableAbilityIds = new List<string>();

    public BattleRunRequest(
        int playerMaxHp,
        int playerCurrentHp,
        int maxEnergy,
        int handSize,
        int enemyHandSize,
        IList<DeckEntry> startingDeck,
        IList<StageData> stages)
    {
        PlayerMaxHp = playerMaxHp;
        PlayerCurrentHp = playerCurrentHp;
        MaxEnergy = maxEnergy;
        HandSize = handSize;
        EnemyHandSize = enemyHandSize;

        if (startingDeck != null)
        {
            StartingDeck.AddRange(startingDeck);
        }

        if (stages != null)
        {
            Stages.AddRange(stages);
        }
    }
}
