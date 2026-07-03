using System;

public static class BattleFlowAdapter
{
    public static BattleRunResult ResolveAuto(BattleRunRequest request, BattleRandomRange randomRange, int maxTurns)
    {
        if (request == null)
        {
            return new BattleRunResult(false, true, 0, "Missing battle request");
        }

        BattleState state = new BattleState();
        TurnController controller = new TurnController(state, randomRange ?? UnityEngine.Random.Range);
        controller.NewGame(
            request.PlayerMaxHp,
            request.MaxEnergy,
            request.HandSize,
            request.EnemyHandSize,
            request.StartingDeck,
            request.Stages);
        state.PlayerHp = Clamp(request.PlayerCurrentHp, 0, request.PlayerMaxHp);
        ApplyTableAbilities(request, state);

        int turnLimit = Math.Max(1, maxTurns);
        for (int turn = 0; turn < turnLimit && !state.GameOver; turn++)
        {
            PlayAffordableCards(controller, state);
            if (!state.GameOver)
            {
                controller.EndTurn();
            }
        }

        bool victory = state.GameOver && state.StageIndex >= state.Stages.Count && state.PlayerHp > 0;
        bool timedOut = !state.GameOver && !victory;
        bool defeat = state.PlayerHp <= 0 || (!victory && state.GameOver) || timedOut;
        string deathReason = timedOut ? "Battle timed out" : (defeat ? "Defeated in battle" : string.Empty);
        return new BattleRunResult(victory, defeat, state.PlayerHp, deathReason);
    }

    private static void ApplyTableAbilities(BattleRunRequest request, BattleState state)
    {
        if (request.TableAbilityIds.Contains("memory_spark"))
        {
            state.PlayerBlock += 2;
        }

        if (request.TableAbilityIds.Contains("echo_call"))
        {
            state.Energy += 1;
        }
    }

    private static void PlayAffordableCards(TurnController controller, BattleState state)
    {
        bool playedCard;
        do
        {
            playedCard = false;
            for (int i = 0; i < state.PlayerDeck.Hand.Count; i++)
            {
                CardData card = state.PlayerDeck.Hand[i];
                if (card != null && state.Energy >= card.Cost)
                {
                    controller.PlayCard(i);
                    playedCard = true;
                    break;
                }
            }
        }
        while (playedCard && !state.GameOver);
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }
}
