using System.Collections.Generic;
using UnityEngine;

public partial class SimpleCardBattle2D
{
    [System.Serializable]
    public class CardDef
    {
        public string Name;
        public int Cost;
        public int Damage;
        public int Block;
        public int Heal;
        public string Description;
        public Color Tint;
        public string ArtPath;

        public CardDef()
        {
        }

        public CardDef(string name, int cost, int damage, int block, int heal, string description, Color tint, string artPath)
        {
            Name = name;
            Cost = cost;
            Damage = damage;
            Block = block;
            Heal = heal;
            Description = description;
            Tint = tint;
            ArtPath = artPath;
        }
    }

    [System.Serializable]
    public class DeckEntry
    {
        public CardDef Card;
        public int Count;

        public DeckEntry()
        {
        }

        public DeckEntry(CardDef card, int count)
        {
            Card = card;
            Count = count;
        }
    }

    [System.Serializable]
    public class EnemyCardDef
    {
        public string Name;
        [Min(0)]
        public int Copies = 2;
        public int Damage;
        public int Block;
        public int Heal;
        public string Description;
        public Color Tint;

        public EnemyCardDef()
        {
        }

        public EnemyCardDef(string name, int damage, int block, int heal, string description, Color tint)
        {
            Name = name;
            Copies = 2;
            Damage = damage;
            Block = block;
            Heal = heal;
            Description = description;
            Tint = tint;
        }
    }

    [System.Serializable]
    public class EnemyDef
    {
        public string Name;
        public int MaxHp;
        public Color Tint;
        public string ArtPath;
        public List<EnemyCardDef> Cards;

        public EnemyDef()
        {
            Cards = new List<EnemyCardDef>();
        }

        public EnemyDef(string name, int maxHp, Color tint, string artPath, params EnemyCardDef[] cards)
        {
            Name = name;
            MaxHp = maxHp;
            Tint = tint;
            ArtPath = artPath;
            Cards = new List<EnemyCardDef>(cards);
        }
    }
}
