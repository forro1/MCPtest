using UnityEngine;

public partial class SimpleCardBattle2D
{
    private void NewGame()
    {
        turnController = new TurnController(battleState, UnityRandomRange, ShowCardEffect);
        if (battleConfig != null)
        {
            turnController.NewGame(battleConfig);
        }
        else
        {
            turnController.NewGame(playerMaxHp, maxEnergy, handSize, enemyHandSize, startingDeck, stages);
        }
        RefreshUi();
    }

    private void PlayCard(int index)
    {
        turnController.PlayCard(index);
        RefreshUi();
    }

    private void EndTurn()
    {
        if (battleState.GameOver)
        {
            NewGame();
            return;
        }

        turnController.EndTurn();
        RefreshUi();
    }

    private int UnityRandomRange(int minInclusive, int maxExclusive)
    {
        return Random.Range(minInclusive, maxExclusive);
    }
}
