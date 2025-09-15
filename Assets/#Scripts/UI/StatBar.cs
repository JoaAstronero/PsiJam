using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    public RectTransform fillRect;
    public float maxBarHeight = 300f;

    public void SetValue(float current, float max)
    {
        if (fillRect == null || max <= 0f) return;
        float ratio = Mathf.Clamp01(current / max);
        var size = fillRect.sizeDelta;
        size.y = ratio * maxBarHeight;
        fillRect.sizeDelta = size;
    }
}