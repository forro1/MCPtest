using System.Collections;
using UnityEngine;

public partial class SimpleCardBattle2D
{
    private void ShowCardEffect(string message, Color color)
    {
        if (effectText == null)
        {
            return;
        }

        if (effectRoutine != null)
        {
            StopCoroutine(effectRoutine);
        }

        effectRoutine = StartCoroutine(PlayEffectRoutine(message, color));
    }

    private IEnumerator PlayEffectRoutine(string message, Color color)
    {
        effectText.text = message;
        effectText.color = Color.Lerp(color, Color.white, 0.25f);
        effectText.canvasRenderer.SetAlpha(0f);
        effectText.rectTransform.anchoredPosition = new Vector2(0f, 18f);
        effectText.rectTransform.localScale = Vector3.one * 0.82f;

        float timer = 0f;
        while (timer < 0.22f)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / 0.22f);
            effectText.canvasRenderer.SetAlpha(t);
            effectText.rectTransform.localScale = Vector3.one * Mathf.Lerp(0.82f, 1.12f, t);
            yield return null;
        }

        timer = 0f;
        while (timer < 0.55f)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / 0.55f);
            effectText.canvasRenderer.SetAlpha(1f - t);
            effectText.rectTransform.anchoredPosition = new Vector2(0f, Mathf.Lerp(18f, 72f, t));
            effectText.rectTransform.localScale = Vector3.one * Mathf.Lerp(1.12f, 1.22f, t);
            yield return null;
        }

        effectText.canvasRenderer.SetAlpha(0f);
        effectRoutine = null;
    }
}
