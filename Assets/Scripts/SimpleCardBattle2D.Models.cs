[System.Serializable]
public class DeckEntry
{
    public CardData Card;
    public int Count;

    public DeckEntry()
    {
    }

    public DeckEntry(CardData card, int count)
    {
        Card = card;
        Count = count;
    }
}
