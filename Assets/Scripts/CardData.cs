using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Battle/Card Data", fileName = "CardData")]
public class CardData : ScriptableObject
{
    public string Name;
    [Min(0)]
    public int Cost;
    public int Damage;
    public int Block;
    public int Heal;
    public List<CardEffectData> Effects = new List<CardEffectData>();
    [TextArea]
    public string Description;
    public Color Tint = Color.white;
    public string ArtPath;

    public List<CardEffectData> RuntimeEffects
    {
        get
        {
            if (Effects != null && Effects.Count > 0)
            {
                return Effects;
            }

            return CardEffectData.FromLegacyValues(Damage, Block, Heal);
        }
    }
}

public enum CardEffectType
{
    Damage,
    Block,
    Heal
}

[System.Serializable]
public class CardEffectData
{
    public CardEffectType Type;
    public int Amount;

    public CardEffectData()
    {
    }

    public CardEffectData(CardEffectType type, int amount)
    {
        Type = type;
        Amount = amount;
    }

    public static List<CardEffectData> FromLegacyValues(int damage, int block, int heal)
    {
        List<CardEffectData> effects = new List<CardEffectData>();
        if (damage > 0)
        {
            effects.Add(new CardEffectData(CardEffectType.Damage, damage));
        }
        if (block > 0)
        {
            effects.Add(new CardEffectData(CardEffectType.Block, block));
        }
        if (heal > 0)
        {
            effects.Add(new CardEffectData(CardEffectType.Heal, heal));
        }

        return effects;
    }
}
