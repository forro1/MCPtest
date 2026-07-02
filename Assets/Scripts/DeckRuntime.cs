using System;
using System.Collections.Generic;

public delegate int BattleRandomRange(int minInclusive, int maxExclusive);

public class DeckRuntime<T>
{
    public readonly List<T> DrawPile = new List<T>();
    public readonly List<T> DiscardPile = new List<T>();
    public readonly List<T> Hand = new List<T>();

    public void ClearAll()
    {
        DrawPile.Clear();
        DiscardPile.Clear();
        Hand.Clear();
    }

    public void AddToDrawPile(T item, int count)
    {
        for (int i = 0; i < count; i++)
        {
            DrawPile.Add(item);
        }
    }

    public bool DrawToHand(BattleRandomRange randomRange, Action onReshuffle)
    {
        if (DrawPile.Count == 0)
        {
            if (DiscardPile.Count == 0)
            {
                return false;
            }

            DrawPile.AddRange(DiscardPile);
            DiscardPile.Clear();
            ShuffleDrawPile(randomRange);
            if (onReshuffle != null)
            {
                onReshuffle();
            }
        }

        Hand.Add(DrawPile[0]);
        DrawPile.RemoveAt(0);
        return true;
    }

    public void MoveHandToDiscard()
    {
        DiscardPile.AddRange(Hand);
        Hand.Clear();
    }

    public void ShuffleDrawPile(BattleRandomRange randomRange)
    {
        if (randomRange == null)
        {
            return;
        }

        for (int i = DrawPile.Count - 1; i > 0; i--)
        {
            int j = randomRange(0, i + 1);
            T temp = DrawPile[i];
            DrawPile[i] = DrawPile[j];
            DrawPile[j] = temp;
        }
    }
}
