using System.Collections.Generic;

public static class TravelerFactory
{
    public const int BaseMaxHp = 50;
    public const int HpPerTrainingLevel = 2;

    public static TravelerRun CreateTraveler(VillageState village)
    {
        int travelerId = village == null ? 1 : village.NextTravelerId();
        int trainingLevel = village == null ? 0 : village.TrainingLevel;
        List<string> startingDeck = new List<string> { "Strike", "Guard", "Spark" };
        TravelerRun traveler = new TravelerRun(travelerId, BaseMaxHp + trainingLevel * HpPerTrainingLevel, startingDeck);

        if (village != null)
        {
            TableAbilityRuntime tableRuntime = TableAbilityRuntime.FromProgress(village.TableProgress);
            traveler.ActiveTableAbilityIds.AddRange(tableRuntime.ActivePassiveIds);
            traveler.ActiveTableAbilityIds.AddRange(tableRuntime.ActiveSkillIds);

            for (int i = 0; i < village.LegacyEchoes.Count; i++)
            {
                if (!village.LegacyEchoes[i].IsRecovered)
                {
                    traveler.VisibleLegacyEchoes.Add(village.LegacyEchoes[i]);
                }
            }
        }

        return traveler;
    }
}
