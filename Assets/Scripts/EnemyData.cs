using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card Battle/Enemy Data", fileName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    public string Name;
    [Min(1)]
    public int MaxHp = 1;
    public Color Tint = Color.white;
    public string ArtPath;
    public List<EnemyActionData> Cards = new List<EnemyActionData>();
}

[System.Serializable]
public class EnemyActionData
{
    public string Name;
    [Min(0)]
    public int Copies = 2;
    public int Damage;
    public int Block;
    public int Heal;
    [TextArea]
    public string Description;
    public Color Tint = Color.white;

    public EnemyActionData()
    {
    }

    public EnemyActionData(string name, int damage, int block, int heal, string description, Color tint)
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
