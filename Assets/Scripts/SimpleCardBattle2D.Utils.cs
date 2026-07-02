using UnityEngine;

public partial class SimpleCardBattle2D
{
    private static Vector2 AnchorStretch()
    {
        return new Vector2(-1f, -1f);
    }

    private static RectTransform RectFrom(RectTransform source)
    {
        return source;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 size)
    {
        if (anchorMin == AnchorStretch() && anchorMax == AnchorStretch())
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return;
        }

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
    }

}
