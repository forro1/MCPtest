using UnityEngine;

public partial class SimpleCardBattle2D
{
    private Sprite LoadCardSprite(CardData card)
    {
        if (string.IsNullOrEmpty(card.ArtPath))
        {
            return null;
        }

        if (cardSprites.TryGetValue(card.ArtPath, out Sprite cachedSprite))
        {
            return cachedSprite;
        }

        Texture2D texture = Resources.Load<Texture2D>(card.ArtPath);
        if (texture == null)
        {
            Debug.LogWarning("未找到卡牌素材: Resources/" + card.ArtPath);
            cardSprites[card.ArtPath] = null;
            return null;
        }
        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        cardSprites[card.ArtPath] = sprite;
        return sprite;
    }

    private Sprite LoadEnemySprite(EnemyData enemy)
    {
        if (string.IsNullOrEmpty(enemy.ArtPath))
        {
            return null;
        }

        if (enemySprites.TryGetValue(enemy.ArtPath, out Sprite cachedSprite))
        {
            return cachedSprite;
        }

        Texture2D texture = Resources.Load<Texture2D>(enemy.ArtPath);
        if (texture == null)
        {
            Debug.LogWarning("未找到敌人素材: Resources/" + enemy.ArtPath);
            enemySprites[enemy.ArtPath] = null;
            return null;
        }

        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        enemySprites[enemy.ArtPath] = sprite;
        return sprite;
    }
}
